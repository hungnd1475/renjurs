   M S B A M L   `   `   `  ����  AIAssignment2.Presentation+    %AIAssignment2.Presentation.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  GoMoku Test��5   	   $
��Auto��6   $
��Auto��6*    � SizeToContent$ WidthAndHeight=�68    � 
ResizeMode$ NoResize=�6W   $��13(�6m    � Icon$ 
Gomoku.png��6{   .��5      �� $��10,10,10,10q�6   .+�5   
   �� ._�5      �� -   5         ��Name$ board��5      $	��500��6$   $	��500��60   5      �� 5	   
   -   5         ��Name$ sizeChooser��5	      $��1��6&    � Minimum$ 11Q�66    � Maximum$ 21Q�6C   $	��400��6P    �� TickPlacement$ TopLeft=�6\   	 �� AutoToolTipPlacement$	 TopLeft=�6t   
 �� TickFrequency$
 2Q�6�    �� IsSnapToTickEnabled . $��0,5,0,5q�6�    � Value$ 13Q�6�   � 5
   
   $��0,5,0,5q�6   $��Center=�6     � ColumnDefinitions 5      �� 5      �� 5      �� 5      .+�5      �� -   5         ��Name$ blackButton��5      $��Start as white��6*   $��25��6C   $��Center=�6O   $��0,5,5,5q�6l   $��90��6}   6�   �� 5      -   5        $ whiteButton��5      $��1��6*   $��Start as black��6:   $��25��6S   $��Center=�6_   $��5,5,5,5q�6|   $��90��6�   6�   �� 5      -   5        $ resetButton��5      $��2��6*   $��Restart��6:   $��25��6L   $��Center=�6X   $��5,5,0,5q�6u   $��90��6�   6�   5      5      5      