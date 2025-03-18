# browser-switch ![browser-switch-icon - 24](https://github.com/user-attachments/assets/398b09ba-acd2-4dde-ac4a-1e7383677b6e)

根据URL规则自动切换不同浏览器打开链接。支持自定义配置。

![browser-switch](https://github.com/user-attachments/assets/923dc20d-bf6b-4707-b20d-ead587a7fb94)

---

## 功能特性 ✨

- 简单易用
- 灵活配置

---

## 安装指南 📦

1. 下载压缩包：[browser-switch-win-64.zip](https://github.com/lilinfangrelax/browser-switch/releases)
2. 解压到**纯英文目录**（不支持中文或其他非英文路径）
3. 双击运行 `browser-switch.exe`
4. 根据软件提示**安装注册表项**，并设置为**默认浏览器应用**

---

## 使用说明 🛠️

点击exe程序即可运行。

通过 `router.txt` 配置文件自定义路由规则（支持正则表达式）：
```
规则名称             :: 正则表达式匹配            :: 浏览器路径                                                    :: 启动参数（选择不同的用户配置）
测试环境             :: uat\.                   :: C:\Program Files\Google\Chrome\Application\chrome.exe        :: --profile-directory="Profile 10"
Github              :: github\.com             :: C:\Program Files\Mozilla Firefox\firefox.exe                 :: 
匹配其他所有地址      :: .*                      :: C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe ::
```


---

## 图标定制 🎨

[图标生成工具](https://icon.vwh.sh/?s=eyJmaWxsQ29sb3IiOiIjRjBGMEYwIiwiZmlsbE9wYWNpdHkiOjEsInNpemUiOjE5MCwicmFkaXVzIjoxMjgsImJnQ29sb3IiOiJsaW5lYXItZ3JhZGllbnQodG8gbGVmdCB0b3AsIHJnYigyNTMsIDI1MSwgMjUxKSwgcmdiKDIzNSwgMjM3LCAyMzgpKSIsInN2Z0NvbG9yIjoiIzM3ODE2YSIsInBvc2l0aW9uIjp7IngiOjAsInkiOjB9LCJyb3RhdGlvbiI6MCwic3Ryb2tlV2lkdGgiOjEuOCwib3BhY2l0eSI6MSwic2NhbGUiOjEsInNoYWRvd0NvbG9yIjoiIzBBMEIwQiIsInNoYWRvd0JsdXIiOjAsInNoYWRvd09mZnNldFgiOjAsInNoYWRvd09mZnNldFkiOjAsInNrZXdYIjowLCJza2V3WSI6MCwiaWNvbkJsdXIiOjAsImJhY2tncm91bmRCbHVyIjowLCJpbm5lclNoYWRvd0NvbG9yIjoiIzBBMEIwQiIsImlubmVyU2hhZG93Qmx1ciI6MSwiaW5uZXJTaGFkb3dYIjoyLCJpbm5lclNoYWRvd1kiOjN9)

---

## 开源协议 📄

MIT 许可证，详见 [LICENSE](LICENSE)。