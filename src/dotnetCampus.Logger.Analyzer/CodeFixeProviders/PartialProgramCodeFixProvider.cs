using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace dotnetCampus.Logger.CodeFixeProviders;

[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public class PartialProgramCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        Diagnostics.DL0101_ProgramIsRecommendedToBePartial.Id
    );

    public override FixAllProvider? GetFixAllProvider() => null;

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        return Task.CompletedTask;
    }
}
