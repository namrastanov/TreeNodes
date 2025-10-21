using FluentValidation;
using TreeNodes.Application.TreeNodes.Commands;

namespace TreeNodes.Application.TreeNodes.Validators;

/// <summary>
/// Validation for renaming nodes.
/// </summary>
public class RenameNodeCommandValidator : AbstractValidator<RenameNodeCommand>
{
    public RenameNodeCommandValidator()
    {
        RuleFor(x => x.NodeId).GreaterThan(0);
        RuleFor(x => x.NewNodeName).NotEmpty().MaximumLength(200);
    }
}


