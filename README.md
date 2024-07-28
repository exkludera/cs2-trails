# cs2-trails
**a plugin that allows players to use custom trails**

<br>

<details>
	<summary>showcase</summary>
	<img src="https://github.com/user-attachments/assets/6d31beec-b2ca-47bc-8f55-fd1eda29faa2" width="125"> <br>
	<img src="https://github.com/user-attachments/assets/3bd99e1b-ffb7-4254-9e7c-9470703e6891" width="175"> <br>
	<img src="https://github.com/user-attachments/assets/1135a673-e19f-4a00-9edc-f4bfc760c45f" width="250"> <br>
	<img src="https://github.com/user-attachments/assets/af7406b0-3911-489c-91e1-3dde79002790" width="300"> <br>
	<img src="https://github.com/user-attachments/assets/7dddc6cc-a0aa-4946-9c49-c5bf6b48ceb1" width="200"> <br>
	the beams in cs2 are unfortunately not as good as they were in csgo :(
</details>

<br>

## information:

### requirements
- [MetaMod](https://cs2.poggu.me/metamod/installation)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [Cruze03/Clientprefs](https://github.com/Cruze03/Clientprefs)

<br>

> [!NOTE]
> inspired by [Nickelony/Trails-Chroma](https://github.com/Nickelony/Trails-Chroma)

<img src="https://media.discordapp.net/attachments/1051988905320255509/1146537451750432778/ezgif.com-video-to-gif_2.gif?ex=66a359f6&is=66a20876&hm=768e346857f44792cf5b2917fe55b525522029ecccac95bb765b881baa6660d7&" width="250">>

## example config
```json
{
  "Prefix": "{red}[{orange}T{yellow}r{green}a{lightblue}i{darkblue}l{purple}s{red}]",
  "PermissionFlag": "@css/reservation",
  "MenuCommands": "trails,trail",
  "CenterHtmlMenu": false,
  "ChatMessages": true,
  "TicksForUpdate": 1,
  "Trails": {
    "1": {
      "Name": "Rainbow Trail",
      "Color": "rainbow"
    },
    "2": {
      "Name": "Particle Trail",
      "File": "particles/ambient_fx/ambient_sparks_glow.vpcf"
    },
    "3": {
      "Name": "Red Trail",
      "Color": "255 0 0",
      "Width": 2,
      "Lifetime": 3
    },
    "4": {
      "Name": "Example Settings",
      "File": "materials/sprites/laserbeam.vtex",
      "Color": "255 255 255",
      "Width": 1,
      "Lifetime": 1
    }
  },
  "ConfigVersion": 1
}
```
