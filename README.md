# 基于 WPF 的自定义帧控制演示项目

[![GitHub stars](https://img.shields.io/github/stars/bitkylin/InteractionByFrames.svg)](https://github.com/bitkylin/InteractionByFrames/stargazers)
![技术](https://img.shields.io/badge/%E6%8A%80%E6%9C%AF-WPF%7CTCP%7CSQLite-brightgreen.svg)
[![GitHub license](https://img.shields.io/badge/许可证-Apache_2-blue.svg)](https://github.com/bitkylin/InteractionByFrames/blob/master/LICENSE)

## 功能说明

基于 WPF 框架和 C# 的 .NET 演示程序，分为客户端和服务端：

- 客户端以自定义帧的形式按序向服务端发送数据

- 服务端根据收到的信息生成多个通道的自定义帧发送至客户端，以实现客户端对服务端的自动化数据采集工作。

- 客户端通过自定义帧控制服务端程序生成自定义数据并进行回传，同时客户端同步将数据存储在本地，方便后续的处理使用。

## 技术特点

- 支持「TCP」和「串口」两种通信方式，TCP协议可用于建立客户端和服务端的连接进行演示。

- 客户端发出一条自定义帧，即可收到所有通道的数据，并进行可视化显示。

- 客户端发出一条自定义帧后，未收到回复，则客户端将重复发送该条信息，直到客户端收到回复。

- 操作客户端时，手工配置的数据、系统自动生成的数据、通过服务端生成的数据等均被临时缓存 SQLite 数据库中。

- 一次工作结束后，客户端软件可将这些数据通过 Json 格式导出为本地文件进行持久化存储。


## 相关文章

- [WPF 下的自定义控件以及 Grid 中控件的自适应](http://www.jianshu.com/p/1526a02f3556)

- [基于 WPF 的酷炫 GUI 窗口的简易实现](http://www.jianshu.com/p/b2b8b0161397)

## [License](https://github.com/bitkylin/InteractionByFrames/blob/master/LICENSE)

> Apache License 2.0
> 
> A permissive license whose main conditions require preservation of copyright and license notices. Contributors provide an express grant of patent rights. Licensed works, modifications, and larger works may be distributed under different terms and without source code.

## 关于我

### 1. 我的主页

名称|二级域名|原始地址
---|---|---
主页|http://bitky.cc|https://bitkylin.github.io
GitHub|http://github.bitky.cc|https://github.com/bitkylin
简书|http://js.bitky.cc|http://www.jianshu.com/u/bd2e386a6ea8
CSDN|http://csdn.bitky.cc|http://blog.csdn.net/llmmll08


### 2. 其他

- 兴趣方向: Java, Android, C#, JavaScript, Node.js, Kotlin 等

- Email: bitkylin@163.com
