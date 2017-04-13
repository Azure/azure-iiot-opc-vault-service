$app_path=(get-item -path ($PSScriptRoot + "\..\..\..\") -verbose).fullname

write-host "Adding pre-commit hook..."

mkdir -Force ($app_path + "\" + ".git\hooks") > $null
del -ErrorAction SilentlyContinue -Force ($app_path + "\" + ".git\hooks\pre-commit")
copy ($app_path + "\project\scripts\git\pre-commit-runner.sh") ($app_path + "\" + ".git\hooks\pre-commit")

write-host "Done."
