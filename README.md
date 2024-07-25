# cs2-trails
**a plugin that allows players to use custom trails**
> request from counterstrikesharp discord

<br>

<details>
	<summary>showcase</summary>
	<img src="https://media.discordapp.net/attachments/1039451649254629406/1266039352040099942/image.png?ex=66a3b287&is=66a26107&hm=491ee4c3ceaecad73e95b973e37966e2b09df2388700cfd04831cc96bb7bac8b&=&format=webp&quality=lossless" width="100"> <br>
	<img src="https://media.discordapp.net/attachments/1039451649254629406/1266039408839102534/image.png?ex=66a3b294&is=66a26114&hm=f306254d49a47b4b8e9714604e430ed2e8277e8e1cb85a2bcea00bfc0719ee4b&=&format=webp&quality=lossless" width="150"> <br>
	<img src="https://media.discordapp.net/attachments/1039451649254629406/1266015953544810506/image.png?ex=66a39cbc&is=66a24b3c&hm=76d6e30e347ded99c9cc9b70cca140d9c325c3a612904858cb5c88f88052cb0c&=&format=webp&quality=lossless" width="200"> <br>
	<img src="https://media.discordapp.net/attachments/1039451649254629406/1266015954073026631/image.png?ex=66a39cbc&is=66a24b3c&hm=4b1ec986649bcba773d92ad3ffc0b94f3a113274ebf8ae1c18e44a484c0d3f2f&=&format=webp&quality=lossless" width="250"> <br>
	<img src="https://media.discordapp.net/attachments/1039451649254629406/1266015954517889115/image.png?ex=66a39cbc&is=66a24b3c&hm=d16ceb95c5ae4e75cf84892ba44440125f4fd65629dc27401097ae5e9cd86874&=&format=webp&quality=lossless" width="250"> <br>
	<img src="https://media.discordapp.net/attachments/1266024113152200809/1266036352282263582/image.png?ex=66a3afbc&is=66a25e3c&hm=aea46d73f97edc442f556da0808c5bd757a172dcf9b37949e8a422f0334c2c4a&=&format=webp&quality=lossless" width="250"> <br>
</details>

<small>*the beams in cs2 are unfortunately not as good as they were in csgo :(*</small>

<br>

## information

> [!CAUTION]
> This plugin needs [Cruze03/Clientprefs](https://github.com/Cruze03/Clientprefs) to work!

> [!NOTE]
> inspired by [Nickelony/Trails-Chroma](https://github.com/Nickelony/Trails-Chroma)

<img src="https://media.discordapp.net/attachments/1051988905320255509/1146537451750432778/ezgif.com-video-to-gif_2.gif?ex=66a359f6&is=66a20876&hm=768e346857f44792cf5b2917fe55b525522029ecccac95bb765b881baa6660d7&" width="250">>

## config
```json
{
  "Prefix": "{red}[{orange}T{yellow}r{green}a{lightblue}i{darkblue}l{purple}s{red}]",
  "Settings": {
    "Width": 0.5,
    "Life": 1,
    "TicksForUpdate": 2,
    "PermissionFlag": "@css/reservation",
    "ChatMessages": true,
    "CenterHtmlMenu": false
  },
  "Commands": {
    "TrailsMenu": [
      "trails",
      "trail"
    ]
  },
  "Trails": {
    "1": {
      "name": "Rainbow Trail",
      "effect": "rainbow"
    },
    "2": {
      "name": "Red Trail",
      "effect": "255 0 0"
    },
    "3": {
      "name": "Green Trail",
      "effect": "0 255 0"
    },
    "4": {
      "name": "Blue Trail",
      "effect": "0 0 255"
    },
    "5": {
      "name": "Custom Trail",
      "effect": "particles/ambient_fx/ambient_sparks_glow.vpcf"
    }
  },
  "ConfigVersion": 1
}
```
