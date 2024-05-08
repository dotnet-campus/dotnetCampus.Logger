using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace dotnetCampus.Logger.Utils.IO;

/// <summary>
/// 从嵌入的资源中寻找源代码。
/// </summary>
internal static class EmbeddedSourceFiles
{
    /// <summary>
    /// 寻找 <paramref name="folderName"/> 文件夹下的源代码名称和内容。
    /// </summary>
    /// <param name="folderName">资源文件夹名称。请以“/”或“\”分隔文件夹。</param>
    /// <returns></returns>
    internal static IEnumerable<EmbeddedSourceFile> Enumerate(string folderName)
    {
        // 资源字符串格式为："{Namespace}.{Folder}.{filename}.{Extension}"
        var desiredFolder = $"{AssemblyInfo.RootNamespace}.{folderName}";
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            var prefix = desiredFolder.Replace('/', '.').Replace('\\', '.') + ".";
            if (resourceName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                var fileName = resourceName.AsSpan().Slice(prefix.Length).ToString();
                using var stream = assembly.GetManifestResourceStream(resourceName)!;
                using var reader = new StreamReader(stream);
                yield return new EmbeddedSourceFile(
                    desiredFolder,
                    folderName,
                    fileName,
                    resourceName,
                    reader.ReadToEnd());
            }
        }
    }
}
