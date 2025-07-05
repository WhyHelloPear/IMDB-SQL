<?php
$i = 0;
$i++;
$cfg['Servers'][$i]['auth_type'] = 'cookie';
$cfg['Servers'][$i]['host'] = 'PeardeVault_DB';
$cfg['Servers'][$i]['port'] = '3306';
$cfg['Servers'][$i]['user'] = 'admin_de_pear';
$cfg['Servers'][$i]['password'] = '{{SECRET_DB_PASSWORD}}';
$cfg['Servers'][$i]['ssl'] = false; //no ssl; only available within internal docker network
$cfg['Servers'][$i]['ssl_verify'] = false;
