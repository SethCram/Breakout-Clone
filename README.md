# Breakout-Clone
- Playable game - https://sethcram.weebly.com/breakout-clone.html
- Built on Unity 2021.3.10f1
- MlAgents tutorial - https://www.youtube.com/watch?v=zPFU30tbyKs
  - start model = "mlagents-learn config/HitBall.yaml --run-id=newBrainID"
    - uses a config file w/ pre-set settings
  - start model from previous brain = "mlagents-learn config/HitBall.yaml --initialize-from=startBrainID --run-id=newBrainID"
  - visualize results - "tensorboard --logdir results"
  
  ## Web-GL 
  - build process:
    - build project for web-gl in unity
    - copy + paste github url of index.html into https://raw.githack.com/ 
    - copy url for production
    - paste url for production into iframe src
      - on weebly and github.io
