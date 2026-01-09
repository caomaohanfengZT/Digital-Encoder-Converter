# 数字编码转换器（WPF）

基于 WPF + MVVM 的原码/反码/补码转换工具。支持多种输入编码，自动计算十进制以及原码、反码、补码的二进制与十六进制结果，界面采用现代化样式并支持自适应布局。

## 功能特性
- 支持原码/反码/补码的二进制与十六进制输入
- 支持原码十进制输入（自动推导位宽）
- 输出统一表格化展示，包含十进制、原码/反码/补码的二进制与十六进制
- 输入即更新（MVVM + INotifyPropertyChanged）
- 界面中文标签，支持自适应布局与最小尺寸约束

## 输入类型
- 原码（2 进制）
- 原码（10 进制）
- 原码（16 进制）
- 反码（2 进制）
- 反码（16 进制）
- 补码（2 进制）
- 补码（16 进制）

## 位宽与编码规则
### 十进制输入
- 计算最小位宽：
  - abs = |value|
  - magBits = max(1, floor(log2(abs)) + 1)
  - minWidth = 1 + magBits
- 默认位宽按 4 位对齐递增：
  - width = 4 * ceil(max(minWidth, 4) / 4)
- 若补码范围不足，继续按 4 位递增：
  - 补码范围 [-2^(width-1), 2^(width-1)-1]

### 二进制/十六进制输入
- 二进制：width = 输入长度
- 十六进制：width = 4 * 输入长度
- 输出若需补齐到 4 的倍数，按符号扩展补齐

## 输入合法性
- 二进制仅允许 0/1
- 十六进制允许 0-9A-F（不区分大小写）
- 十进制允许 '-' 前缀
- 输入为空或非法字符时会显示错误信息

## 输出说明
- 输出固定位宽
- 十六进制输出使用大写
- 统一归一化负零为 +0

## 环境要求
- Windows 10/11
- .NET 8 SDK

## 运行方式
```powershell
dotnet build E:\Project\Personal\Converter\Converter.sln
dotnet run --project E:\Project\Personal\Converter\Converter.App
```

## 项目结构
- `Converter.App`：WPF 应用
  - `Services/EncodingConverter.cs`：核心转换逻辑
  - `ViewModels/MainViewModel.cs`：MVVM 绑定
  - `MainWindow.xaml`：界面布局

## 截图
如需截图，请运行程序后自行补充。
