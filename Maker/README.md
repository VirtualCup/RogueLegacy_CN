# 文本制作   
1. 打开 VS 开发者控制台。
2. 执行 Commands 文件里的命令，通过 Resx 文件生成相应的 Dll 文件。

# 字库制作
1. 利用 Python 程序读取 Resx 文件里的文本生成相应的 Spritefont 文件。
2. 利用 XNAContentCompiler 按照 Spritefont 文件的逻辑生成 Xnb 字库文件。

# 程序制作
1. 对 DS2DEngine.dll 进行逆向工程，修改 TextObj 类后重新编译。