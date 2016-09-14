# SwordGame

Trello: https://trello.com/b/s2MO2QKj/sword-game

This is a game about building the biggest sword.

### Name Candidates
- Swordplay: Get Huilt Get Built
- Sword Forager
- ??? NEEDS MORE



### Game Structure

![Alt text](http://www.plantuml.com/plantuml/img/ZLJBJiCm4BpxAwAUW2X8N54LWH2FIa0hbSVPnAl6mh6Zn26gW7zdrqxiG2dYrEnZPcUzevwrbZUsgnHZXU9jcvpNjP85jzBev9sb6rv1SffSumzBEucVZuixhla66fPgiBrfNZ6vr4fgkFSnIzT4IfY3Goo-6IdOvpY0qiHSAxw5fiNwhMpjVEN2dAMWeP8GyfSORZ4jTTtPa5rGT6T4fo17Qi_yR1ebYDgyu5Q5YYJBCP577HSbRtXXeI542LpP4jiNsg8qqqht0AV9rHCKrk2kv-ADw-BRGfx72oUI5fSNM5xJxny8jn811A_m8SUfl_GF04T8W1xafhGZODrBlOdj-mSw_0fFiilnNmdXwkvqN8EYnyKx5w_p5ONn32uCRG_RckXNupTBbpNeYUeBPTh9n7HTclx0s1973R1Okv42zjI2qearI4HOAVPDBPu9q-iizmR_hH3j7fzWTZ9BigxEZf9PnVLsvgKmvspAvzwAZ0tUo3xoQ3Vcp_yzvx6xl0w_f-Xu9hLOcJp1AE7uUDZHJmzXa3Yiwxth3KTDWmAVs94GZCMYnNwYN7AUPPFcdqEdUjaPQ87_Hry0)

Here dotted lines represent "references" which don't actually exist in C#. It simply means that there could be an interface here, but I avoid creating interfaces because it contributes to class hell.

Pointing arrows represent ownership over objects. This has less meaning in C# where there isn't memory management, but it does imply that the owner will call some sort of instantiation method on the owned class.
