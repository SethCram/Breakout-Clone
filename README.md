# Breakout-Clone
A modern spinoff of the classic Breakout arcade game built in Unity3d. AI built with ml-agents plays the game during periods inactivity on the title screen. 

[See the presentation](https://docs.google.com/presentation/d/1HEdi159ltVG2eTYerhlTsmcbYpWvM3ygljTGu5KgJrk/edit?usp=sharing)

## Play the game
[![Brick Breaker Title](/images/brick_breaker.PNG)](https://sethcram.github.io/?game=2#games)

## Developer Notes
- Built on Unity 2021.3.10f1
- MlAgents tutorial - https://www.youtube.com/watch?v=zPFU30tbyKs
  - start model = `mlagents-learn config/HitBall.yaml --run-id=newBrainID`
    - uses a config file w/ pre-set settings
  - start model from previous brain = `mlagents-learn config/HitBall.yaml --initialize-from=startBrainID --run-id=newBrainID`
  - visualize results - `tensorboard --logdir results`
  
## Deployment Notes
### Web-GL 
- build process:
  - build project for web-gl in unity
  - copy + paste github url of index.html into https://raw.githack.com/ 
  - copy url for production
  - paste url for production into iframe src
    - on weebly and github.io
