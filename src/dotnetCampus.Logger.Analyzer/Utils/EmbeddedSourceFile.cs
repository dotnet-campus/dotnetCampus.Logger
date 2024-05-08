namespace dotnetCampus.Logger.Utils;

/// <summary>
/// 嵌入的文本资源的数据。
/// </summary>
/// <param name="Namespace">文件的命名空间。</param>
/// <param name="RelativeDirectoryPath">文件相对于项目根目录所在的文件夹的路径。</param>
/// <param name="FileName">文件的名称（含扩展名）。</param>
/// <param name="EmbeddedName">文件在嵌入的资源中的名称。</param>
/// <param name="Content">文件的文本内容。</param>
internal readonly record struct EmbeddedSourceFile(
    string Namespace,
    string RelativeDirectoryPath,
    string FileName,
    string EmbeddedName,
    string Content);
