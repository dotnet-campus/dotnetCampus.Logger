using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace dotnetCampus.Logger.Analyzer.Utils;

/// <summary>
/// 从嵌入的资源中寻找源代码。
/// </summary>
internal static class EmbeddedSourceFiles
{
    /// <summary>
    /// 寻找 <paramref name="folderName"/> 文件夹下的源代码名称和内容。
    /// </summary>
    /// <param name="folderName">资源文件夹名称。</param>
    /// <returns></returns>
    internal static IEnumerable<EmbeddedSourceFile> Enumerate(string folderName)
    {
        // 资源字符串格式为："{Namespace}.{Folder}.{filename}.{Extension}"
        var desiredFolder = $"{typeof(EmbeddedSourceFiles).Namespace}.{folderName}";
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (resourceName.StartsWith(desiredFolder, StringComparison.OrdinalIgnoreCase))
            {
                using var stream = assembly.GetManifestResourceStream(resourceName)!;
                using var reader = new StreamReader(stream);
                yield return new(resourceName, reader.ReadToEnd());
            }
        }
    }
}
