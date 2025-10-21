using FluentValidation;
using TreeNodes.Application.TreeNodes.Commands;

namespace TreeNodes.Application.TreeNodes.Validators;

/// <summary>
/// Validation for creating nodes.
/// </summary>
public class CreateNodeCommandValidator : AbstractValidator<CreateNodeCommand>
{
    public CreateNodeCommandValidator()
    {
        RuleFor(x => x.TreeName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NodeName).NotEmpty().MaximumLength(200);
    }
}


