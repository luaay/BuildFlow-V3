using FluentValidation;

namespace BuildFlow.Documents.Application.Features.Documents.CreateDocument;

public sealed class CreateDocumentValidator
    : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Document title is required.")
            .MaximumLength(200);

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(300);

        RuleFor(x => x.FilePath)
            .NotEmpty().WithMessage("File path is required.");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0).WithMessage("File size must be greater than zero.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required.");
    }
}