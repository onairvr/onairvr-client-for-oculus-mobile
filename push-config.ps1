$config = $args[0]
$packageName = "kr.co.clicked.onairvrapp.ovr"

$dest = "/storage/emulated/0/Android/data/$packageName/files/config.json"

adb push $config $dest
