   M S B A M L   `   `   `  ����  KlAkEnum    KlAkEnum.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5          �Name$  
SrvTreeWnd��5   	    � Title$. (Серверы Kaspersky Security Center��5   	   $	��350��63   $��788.958��6@   .��5      �  � ColumnDefinitions 5	   
   �� 5
      $
��275* �6   �� 5      $
��Auto �6   �� 5      $
��480* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      .+�5   
   o� -   5         o�Name$ MainMenu��5      $��3��6!   .@�5      m� -   5         m�Name$ MenuFile��5      $��Файл��6)   .@�5      m� -   5        $ 
MenuAddSrv��5      $#��Добавить сервер��6/   6H   �� 5      m� 5      -   5        $ MenuExit��5      $��
Выход��6-   6<   5      m� 5      $��Действия��6   .@�5      m� -   5        $ MenuConnect��5      $��Подключиться��60   {�.  6X   m� 5      -   5        $ MenuDisconnect��5      $��Отключиться��63   {�.  6Z   �� 5      m� 5      -   5        $ 
MenuBrowse��5      $"��Обозреватель KSC��6/   {�.  6[   m� 5      -	   5        $ MenuTest��5      $
��Test��6-   6;   5      5      R� 5    
   -
   5         R�Name$ SrvTree��5       $��1��6$   61   �� 5!   
   $��1��6   $��1��6$   $��2��64   $��Center=�6>   R� 5"   
   -   5        $ SrvInfo��5"      $��1��6$   $��2��61   5#      5$      