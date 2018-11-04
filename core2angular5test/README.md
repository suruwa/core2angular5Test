`dotnet ef migrations add "Initial" -o "Data/Migrations"`

parametr -o -> katalog gdzie zostaną zapisane wygenerowane migracje

`dotnet ef database update` - update bazy, uruchomienie migracji

HMR nie wygląda by działało, albo działa nieprzewidywalnie - naprawić