# Tireslide
1. [Opis](#Opis)
2. [Założenia gry](#Założenia-gry)
3. [Główne mechaniki](#Główne-mechaniki)
    - [Generator tras](#Generator-tras)
    - [System punktowy](#System-punktowy)
4. [Sterowanie](#Sterowanie)
    - [Klawiatura](#Klawiatura)
    - [Gamepad](#Gamepad-(PS/XBOX))
5. [Użyte assety](#Użyte-assety)

## Opis
Gatunek: wyścigowa

Gracz ma za zadanie zdobyć jak najwiekszą ilość punktów w wyznaczonym czasie na generowanych losowo trasach. Przy pomocy samochodu do driftu zdobywa punkty poprzez wykonywanie poślizgów lub przejeżdzanie przez bramki lub zderzanie się z obiektami za które można zdobyć premie punktową.

## Założenia gry
Za wykonywane poślizgi są zdobywane punkty. Na ilość punktów zdobywtych wpływa kąt poslizgu oraz prędkość. Jeśli gracz zderzy się ze ścianą punkty za poślizg są anulowane. Zdobywane są tylko jeśli pomyślnie zakończy poślizg.

Na trasie można napotkać premie punktowe w postaci bramek oraz czerwonych pudełek. Za przejazd przez bramkę lub zderzenie się z pudełkami gracz jest nagradzany natychmiastowo punktami.

Gracz musi ukończyć trasę w wyznaczonym limicie czasowym. Jeśli limit czasu upłynie od wyniku punktów są odejmowane punkty wraz z upływem czasu. Także gracz nie może zdobywać dodatkowych punktów.

Istnieje mnożnik punktów, który można zwiększyć przejeżdzając przez bramke mnożnika. Mnożnik zostaje zwiększony o 1 na 10 sekund. Wszystkie punkty, które są zdobywane przez gracza są mnożone przez aktualny mnożnik. Po upływie czasu mnożnik wynosi 0 co skutkuje brakiem możliwości zdobywaniem punktów tak jak to było wspomniane wyżej.

## Główne mechaniki
### Generator tras
klocki
łączniki
hitboxy sprawdzajace nakladanie sieklockow
### System punktowy
drift 
bonus bramka
x2 bramka
pudelka

## Sterowanie
### Klawiatura
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/Arrow_Up_Key_Dark.png">/<img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/W_Key_Dark.png"> - przyspieszenie</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/Arrow_Left_Key_Dark.png">i<img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/Arrow_Right_Key_Dark.png">/<img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/A_Key_Dark.png">i<img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/D_Key_Dark.png"> - kierowanie</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/Arrow_Down_Key_Dark.png">/<img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/S_Key_Dark.png"> - hamulec/wsteczny</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/Space_Key_Dark.png">/<img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/Ctrl_Key_Dark.png"> - hamulec ręczny</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/Del_Key_Dark.png"> - restart</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/Keyboard_Mouse/Dark/Esc_Key_Dark.png"> - pauza</td></tr></table>

### Gamepad (PS/XBOX)
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/PS4/PS4_R2.png">/<img width="30px" src="Key_Prompts/Xbox One/XboxOne_RT.png"> - przyspieszenie</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/PS4/PS4_Left_Stick.png">/<img width="30px" src="Key_Prompts/Xbox One/XboxOne_Left_Stick.png"> - kierowanie</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/PS4/PS4_L2.png">/<img width="30px" src="Key_Prompts/Xbox One/XboxOne_LT.png"> - hamulec/wsteczny</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/PS4/PS4_R1.png">/<img width="30px" src="Key_Prompts/Xbox One/XboxOne_RB.png"> - hamulec ręczny</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/PS4/PS4_Triangle.png">/<img width="30px" src="Key_Prompts/Xbox One/XboxOne_Y.png"> - restart</td></tr></table>
- <table><tr><td valign="middle"><img width="30px" src="Key_Prompts/PS4/PS4_Options.png">/<img width="30px" src="Key_Prompts/Xbox One/XboxOne_Menu.png"> - pauza</td></tr></table>


## Co może być dodane
 - Tabele wyników graczy dla każdej z tras
 - Personalizacja wyglądu samochodu, zarówno malowanie jak i model
 - Edytor tras
 - Więcej elementów do generatora tras (skoki, rampy, droga z dziurami etc.)
 - Więcej obiektów do generacji na trasie (inne pudełka, bramki, strefy driftu etc)


## Użyte assety
 - [dźwięk opon](https://randyol.home.xs4all.nl/wavgeluiden/wav_geluiden.htm)
 - [dźwięk startu/odliczania](https://freesound.org/people/JustInvoke/sounds/446142/)
 - <img style="height: 60px; vertical-align: middle;" src="driftCar.png">
 - [dźwięk silnika](https://assetstore.unity.com/packages/audio/sound-fx/transportation/rotary-x8-free-engine-sound-pack-106119)
 - [wykonanie elementów generatora przy pomocy bezier path creator](https://assetstore.unity.com/packages/tools/utilities/b-zier-path-creator-136082)
 - [dźwięk kostek](https://freesound.org/people/AxelSpeller/sounds/369746/)
 - [dźwięk barmki x2 oraz bonus](https://freesound.org/people/Tetoszka/sounds/541499/)
 - [dźwięk uderzenia w ściane](https://freesound.org/people/thecoolcookie17/sounds/573047/)
 - [dźwięk zdobycia pkt za drift/skok](https://freesound.org/people/qubodup/sounds/60013/)
 - [dźwięk podczas zdobywania pkt za drift/skok](https://freesound.org/people/Joao_Janz/sounds/482653/)
 - [dźwięk guzików menu](https://freesound.org/people/suntemple/sounds/253172/)
 - [skrypt śladów opon](https://github.com/Nition/UnitySkidmarks)
 - [konfetti1](https://freesound.org/people/Breviceps/sounds/458398/) + [konfetti2](https://freesound.org/people/themfish/sounds/45825/)

[Dlaczego TextMeshPro jest w repozytorium i nie jest ignorowane](https://github.com/game-ci/unity-actions/issues/62)
