# PLC 课程设计 - 密码锁

## PLC

PLC 程序打包为 [src/PLCSmartLock.zip](src/PLCSmartLock.zip)

PLC 程序在 TIA Portal 中打印为 [src/PLCSmartLock.pdf](src/PLCSmartLock.pdf)

开发环境
- Siemens TIA Portal V16

型号
- PLC 型号： Siemens S7-1200
- PLC CPU： CPU 1215C AC/DC/Rly (6ES7 215-1HG40-0XB0)

外围设备
- 数字量输入： I0.0 - I0.7，I1.0 - I1.4
- 模拟量输入： AI0
- 数字量输出： Q0.0 - Q0.7

## 上位机

上位机程序在 [src/PLCHost](src/PLCHost)
请直接打开解决方案 [PLCSmartLock.sln](PLCSmartLock.sln)

运行环境
- ASP.NET Core 6.0 Runtime

开发环境
- .NET SDK 6.0.x


## 许可证

本仓库使用 [DO WHAT THE F**K YOU WANT TO PUBLIC LICENSE(WTFPL, 
你TM爱干啥干啥)](./LICENSE) 许可证授权
