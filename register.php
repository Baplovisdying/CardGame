<?php

    $con = mysqli_connect('localhost','root','root','testdatabase','3307');

    if(mysqli_connect_errno())
    {
        echo "1:数据库连接失败 ";   //错误代码 1 数据库连接失败
        exit();
    }

    $username = $_POST["user_name"];
    $password = $_POST["user_password"];

    $nameCheckQuery = "SELECT user_name FROM test_table WHERE user_name ='".$username."';";
    $nameCheck = mysqli_query($con,$nameCheckQuery) or die("2:用户名检测失败");    //错误代码 2 用户名检测失败

    if(mysqli_num_rows($nameCheck)>0)
    {
        echo "3:用户名已存在";  //错误代码 3 用户名已存在
        exit();
    }

    $salt = "\$5\$rounds=5000\$" . "CrazyThursdayVFiftyYuanToMe" . $username . "\$"; 
    $hash = crypt($password,$salt);
    $insertUserQuery = "INSERT INTO test_table (user_name,user_salt,user_hash) VALUES ('". $username . "','". $salt . "','". $hash . "');";
    $insertNewUser = mysqli_query($con,$insertUserQuery) or die("4:注册用户失败");   //错误代码 4 注册用户失败

    echo("0");

?>