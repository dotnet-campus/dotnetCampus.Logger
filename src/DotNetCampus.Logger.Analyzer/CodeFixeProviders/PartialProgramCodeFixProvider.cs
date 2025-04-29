using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.Logger.CodeFixeProviders;

[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public class PartialProgramCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        Diagnostics.DL0101_ProgramIsRecommendedToBePartial.Id
    );

    public override FixAllProvider? GetFixAllProvider() => null;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        if (root is null)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
        if (semanticModel is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var cds = (ClassDeclarationSyntax)root.FindNode(diagnosticSpan);
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: string.Format(DL1001_Fix, cds.Identifier.Text),
                    createChangedDocument: c => AddPartialModifierAsync(context.Document, cds, c),
                    equivalenceKey: nameof(DL1001_Fix)),
                diagnostic);
        }
    }

    private async Task<Document> AddPartialModifierAsync(Document document, ClassDeclarationSyntax classDeclarationNode, CancellationToken cancellationToken)
    {
        var newClassDeclarationNode = classDeclarationNode.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root is null)
        {
            return document;
        }

        var newRoot = root.ReplaceNode(classDeclarationNode, newClassDeclarationNode);
        return document.WithSyntaxRoot(newRoot);
    }
}
