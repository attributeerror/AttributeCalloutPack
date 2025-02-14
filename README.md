# AttributeCalloutPack

This is a simple callout pack that I developed for FivePD. Right now, it only contains two callouts: **Abandoned Vehicle** and **Burglary**. These callouts are static in their behaviour, but over time I hope to make this configurable to customise how you experience these callouts.

## Abandoned Vehicle Callout

Dispatch notifies you of a report of an abandoned vehicle. There are multiple scenarios available within this callout:

1. The vehicle has been abandoned for a couple of days, and no-one has been seen returning to the vehicle.
2. The vehicle has been involved in a front-end collision. Everyone involved fled the scene, and no-one saw the collision itself. Make sure there's nothing suspicious about the vehicle, and get it towed.
3. The vehicle has been involved in a rear-end collision. Everyone involved fled the scene, and no-one saw the collision itself. Make sure there's nothing suspicious about the vehicle, and get it towed.
4. The vehicle has been abandoned, but it looks as if it's broken down. The vehicle can't be moved, and could be blocked the road. Tow the vehicle, and notify its owner.
5. The vehicle has been abandoned but it appears that some of the tires has been slashed/deflated. This could be on purpose, or they could have a slow puncture, who knows?

### Planned Features

* Add the vehicle information to the police computer.
* Have the owner's information available in the police computer.
* Add suspicious items to the vehicle's inventory.
* Possibility of having injured/dead peds laying near the vehicle, or slumped over the steering wheel.
* Possibility of witnesses having seen the incident and helping you with your investigation.

## Burglary Callout

Dispatch notifies you of a report of someone attempting to break into a house. It's your job to respond to the scene and see if there really was a break-in, or if it's a hoax call or a paranoid neighbour! When you get on scene, you will be able to talk to the suspect and ask them some questions. There are multiple variations of the questions between yourself and the suspect, so you shouldn't get the same dialogue over and over again!

### Default Controls

* E - Talk to the subject.

### Planned Features

* Turn the diagloue into an interaction menu, allowing for more possibilities in the conversation.
* Chance that the suspect attacks you, depending on the weapon they possess.
* Chance that the suspect flees on foot, depending on where you take the conversation.
* Chance that the suspect calls for backup, and flees in a vehicle. This vehicle could also attempt to perform a drive-by as they speed off!
* Chance that the suspect is no longer in the area (gone on arrival).
* Chance that there will be people on scene that argue with you, or the suspect, or both.

## Contributing

If you wish to contribute to the development of these callouts, you can make your own fork and get started right away!  
Please take a look at the [contributing guide](./CONTRIBUTING.md) for detailed information on how to get started.

### Join me in Discussions

I use GitHub Discussions to talk about all sorts of topics related to my callout pack! For example, if you'd like help troubleshooting a PR or have a suggestion, join us in the [discussions](https://github.com/attributeerror/AttributeCalloutPack/discussions).