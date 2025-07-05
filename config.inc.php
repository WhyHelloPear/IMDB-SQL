<?php
$i = 0;
$i++;
$cfg['Servers'][$i]['auth_type'] = 'cookie';
$cfg['Servers'][$i]['host'] = 'IMDB_DB';
$cfg['Servers'][$i]['port'] = '3306';
$cfg['Servers'][$i]['user'] = 'imdb_admin';
$cfg['Servers'][$i]['password'] = '{{SECRET_DB_PASSWORD}}';
$cfg['Servers'][$i]['ssl'] = false; //no ssl; only available within internal docker network
$cfg['Servers'][$i]['ssl_verify'] = false;
