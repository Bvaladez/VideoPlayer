   M S B A M L   `   `   `  ����   BardsVideoPlayer!    MediaElementDemo.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Bards Media Playa��$	��500��$	��700�����#404040
�������.���  � ColumnDefinitions �� �� $��80 � � RowDefinitions �� �� $��22 �.+��� 
�������$��1q�._�r� -   $��Media��$��2q� r� LoadedBehavior$ Manual=��� $��1������ .R��  �� $
��Auto ��� $��* ��� $
��Auto �.@��� .���� -   $��TimeDisplay��$��1��
00::00::00�� $��1��$��Stretch=�.���� -   $��SliderPrgBar���� $��1��$
��Auto��.+��� -   $��PlayPauseButton��.��Play�� -   .��Stopd� -   $��0 5 0 0q�.��Mute�� $��10(�Volume�� -   $��VolumeSlider�� � Value$
 0.75Q� � Maximum$ 1Q��� $��10(�Speed�� -   $��SpeedSlider��$ 1Q� � Minimum$ 0Q�$ 4Q� �� TickPlacement$
 Both=� �� IsSnapToTickEnabled$
 True��	 �� Ticks$	 0, 0.25, 0.5, 1, 2, 4R��� $��15���� $��5��
�������
���   ��� -	   $��SkipBlurButton��.��Blur�� $��15���� $��10(�
Start Time�� -
   $��StartTimeInput���� $��10(�
End Time�� -   $��EndTimeInput���� -   $��0 5 0 0q�.��Add�� $��10(�$��Center=�$��0 5 0 0q�
Media File�� 
�������$��2���� -   $��FileTextDisplay��$��10(�$��Center=�$��Oblique$�$��0 5 0 0q��� $��9(�$��Center=�$��0 5 0 0q�Blocked Times File�� 
�������$��2���� -   $��BlockFileTextDisplay��$��10(�$��Center=�$��Oblique$�$��0 5 0 0q��� -   $��1��$��1��.��Load