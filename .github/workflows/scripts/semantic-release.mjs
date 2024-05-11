import * as childProcess from 'child_process';
import core from '@actions/core';
import semanticRelease from 'semantic-release';

const setGitConfigSafeDirectory = () => {
	try {
		core.debug(`Enabling GitHub workspace as a git safe directory`);

		const spawn = childProcess.spawnSync(
			'git',
			['config', '--global', '--add', 'safe.directory', process.env.GITHUB_WORKSPACE],
		);

		if (spawn.status !== 0) {
			throw new Error(spawn.stderr);
		}

		core.debug(`Set ${process.env.GITHUB_WORKSPACE} as a safe directory.`);
	} catch (err) {
		core.debug(`Error setting ${process.env.GITHUB_WORKSPACE} as a safe directory.`);
		throw err;
	}
};

async function run() {
	const branches = [
		'main',
		{
			name: 'alpha',
			prerelease: true
		},
		{
			name: 'beta',
			prerelease: true
		}
	];
	const plugins = [
		[
			"@semantic-release/commit-analyzer",
			{
				"releaseRules": [
					{ "breaking": true, "release": "major" },
					{ "revert": true, "release": "patch" },
					{ "type": "feat", "release": "minor" },
					{ "type": "fix", "release": "patch" },
					{ "type": "chore", "release": "patch" },
					{ "type": "perf", "release": "patch" },
					{ "type": "refactor", "release": "patch" },
					{ "type": "revert", "release": "patch" },
					{ "type": "test", "release": "patch" },
					{ "type": "npm", "release": "patch" },
					{ "type": "build", "scope": "deps", "release": "patch" },
					{ "scope": "no-release", "release": false }
				],
				"parserOpts": {
					"noteKeywords": ["BREAKING CHANGE", "BREAKING CHANGES"]
				}
			}
		],
		[
			"@semantic-release/release-notes-generator",
			{
				"parserOpts": {
					"noteKeywords": ["BREAKING CHANGE", "BREAKING CHANGES"]
				},
				"writerOpts": {
					"commitsSort": ["subject", "scope"]
				}
			}
		]
	];
	const tagFormat = "v${version}";

	core.debug(`branches: ${branches}`);
	core.debug(`plugins: ${plugins}`);
	core.debug(`tag-format: ${tagFormat}`);

	setGitConfigSafeDirectory();

	const options = {
		branches,
		plugins,
		tagFormat,
	};

	core.debug(`Options before cleanup: ${JSON.stringify(options)}`);

	Object.keys(options).forEach(
		key => (options[key] === undefined || options[key] === '') && delete options[key]
	);

	core.debug(`Options after cleanup: ${JSON.stringify(options)}`);

	const result = await semanticRelease(options);
	if (!result) {
		core.debug(`no release published`);

		core.exportVariable('NEW_RELEASED_PUBLISHED', 'false');
		core.setOutput('new-release-published', 'false');
		return;
	}

	const { lastRelease, nextRelease, commits } = result;

	core.debug(
		`Published ${nextRelease.type} release version ${nextRelease.version} containing ${commits.length} commits.`,
	);

	if (lastRelease.version) {
		core.debug(`The last release was "${lastRelease.version}".`);
	}

	const { version, notes, type, channel, gitHead, gitTag, name } = nextRelease;
	const [ major, minor, patch ] = version.split(".");

	// Export environment variables
	core.exportVariable('NEW_RELEASE_PUBLISHED', 'true');
	core.exportVariable('RELEASE_VERSION', version);
	core.exportVariable('RELEASE_MAJOR', major);
	core.exportVariable('RELEASE_MINOR', minor);
	core.exportVariable('RELEASE_PATCH', patch);
	core.exportVariable('RELEASE_NOTES', notes);
	core.exportVariable('RELEASE_TYPE', type);
	core.exportVariable('RELEASE_CHANNEL', channel);
	core.exportVariable('RELEASE_GIT_HEAD', gitHead);
	core.exportVariable('RELEASE_GIT_TAG', gitTag);
	core.exportVariable('RELEASE_NAME', name);

	// Export step outputs
	core.setOutput('new-release-published', 'true');
	core.setOutput('release-version', version);
	core.setOutput('release-major', major);
	core.setOutput('release-minor', minor);
	core.setOutput('release-patch', patch);
	core.setOutput('release-notes', notes);
	core.setOutput('release-type', type);
	core.setOutput('release-channel', channel);
	core.setOutput('release-git-head', gitHead);
	core.setOutput('release-git-tag', gitTag);
	core.setOutput('release-name', name);
}

run().catch(core.setFailed);