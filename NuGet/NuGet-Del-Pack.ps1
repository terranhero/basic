﻿$path= Get-Location
if(Test-Path $path"\NuGet.exe")
{
    #Start-Process NuGet.exe "setApiKey 7a3459c0-d9ce-4c81-aa2f-3a9faf36a81d"
    $nugetApiKey = "oy2pylx5e6d5fvyzxdgqr2tfyhddl66udn5w3to2oeb35m"
    $nugetSource = "https://www.nuget.org/api/v2/package"
    $parentPath = Split-Path -Parent $path
    $binPath=$parentPath+"\bin\netstandard2.0\"
    if(Test-Path $path"\Package"){Remove-Item $path"\Package\*.*"}
    else{$null = New-Item Package -type directory}
    Write-Host "开始删除NuGet包"
    Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13552 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13589 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13588 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13587 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13586 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13585 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13584 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13583 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13582 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13581 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13580 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13579 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13578 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13577 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13576 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13575 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13574 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13573 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13572 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13571 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13570 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13568 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13567 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13566 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13564 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13563 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13561 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13560 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13559 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13558 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13557 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13556 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13555 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13554 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13552 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13551 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13550 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13549 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13548 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13545 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13544 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13543 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13542 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13541 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13540 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13539 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13537 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13536 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13535 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13534 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13533 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13532 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13531 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13530 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13529 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13528 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13527 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13526 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13525 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13524 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13523 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13522 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13521 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13520 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13519 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13518 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13517 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13516 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13515 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13513 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13512 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13511 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13510 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13509 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13508 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13507 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13506 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13505 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13504 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13503 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13502 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13501 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13500 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13499 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13498 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13497 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13496 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13495 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13494 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13493 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13492 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13490 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13489 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13486 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13485 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13484 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13483 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13482 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.7.13450 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13480 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13479 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13478 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13477 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13476 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13475 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13474 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13473 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13472 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13471 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13470 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13469 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13468 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13467 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13466 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13465 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13464 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13463 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13462 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13461 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13460 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13459 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13458 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13457 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13456 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13455 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13454 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13453 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13452 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13451 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13450 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13449 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13448 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13447 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13446 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13445 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13444 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13443 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13442 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13441 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13440 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13439 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13438 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13437 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13436 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13435 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13434 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13433 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13432 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13431 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13430 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13429 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13427 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13426 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13424 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13423 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.5.13422 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13419 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13418 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13417 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13416 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13415 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13414 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13413 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13412 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13411 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13410 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13409 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13408 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13407 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13406 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13405 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13404 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13403 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13402 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.13401 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1339 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1338 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1337 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1336 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1335 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1334 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1332 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1331 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1329 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1328 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1327 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1326 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1325 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1324 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1323 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1322 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1321 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1320 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1319 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1318 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1317 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1316 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1315 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1313 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1312 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1311 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1310 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1309 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1307 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1306 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1305 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1304 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1303 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1302 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1301 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1300 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1299 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1298 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1297 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1296 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1295 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1294 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1293 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1292 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1291 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1290 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1289 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1288 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1287 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1286 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1285 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1284 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1283 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1282 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1281 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1280 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1279 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1278 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1277 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1276 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1275 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1274 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1273 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1272 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1271 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1270 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1269 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1268 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1267 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1266 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1265 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1264 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1263 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1262 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1261 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1260 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1259 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1258 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1257 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1256 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1255 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1254 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1253 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1252 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1251 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1250 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1249 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1248 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1246 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.DataAccess 4.0.2016.1245 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
    Write-Host "删除NuGet包结束"
}
cmd /c "pause"
#Remove-Item -Recurse -Force $path"\Package"