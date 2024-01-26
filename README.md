<div align="center">

# Dots & Dashes (Beta-1)

<img width="752" alt="Screenshot 2024-01-26 at 1 03 59 AM" src="https://github.com/MLivanos/AdversarialDotsAndDashes/assets/59032623/bf8b85de-758d-491c-955f-da273a2b3c70">

<p><a href="https://mlivanos.github.io/AdversarialDotsAndDashes"><img alt="Static Badge" src="https://img.shields.io/badge/PLAY-black?style=for-the-badge&logo=unity&logoColor=white&labelColor=black&color=black&link=mlivanos.github.io%2FSearchAlgorithmVisualizer%2F" style="width:225px;"/></a></p>

</div>

 A demonstration of the adversarial search process using the classic pencil and paper game Dots and Dashes (or Dots and Boxes. Take your pick of the name). Play against different AI agents and see how they create/explore the game tree.

 One of the most interesting features of this tool is the use of the "Visualize Strategy" button. Hovering over this will show Minimax's logic for why they chose the move they chose. It will display the best move for each player that they've discovered from building the game tree. Opacity denotes how far ahead the algorithm is looking - for example, in the picture above, a minimax player (green) looking four moves ahead believes that the human player (pink) will claim the bottom-left box (see 1a annotated picture below) and play the bottom-middle vertical line (1b). Then, green will claim the middle two boxes on the bottom (2a) and play the second-column third-row vertical line (2b), letting the human player take the three boxes on the top left (3a) before yielding the final line (3b). One can tell the order of the moves because moves planned in the future have low-opacity. A quick analysis of this board reveals that this is the optimal move choice.

 <div align="center">
  
<img width="752" alt="Screenshot 2024-01-26 at 1 03 59 AM copy" src="https://github.com/MLivanos/AdversarialDotsAndDashes/assets/59032623/0871b54e-a6b1-4c41-bd02-5ef8c5518d2f">

Annotated picture exemplifying the potential of the "Visualize Strategy" button.

</div>
