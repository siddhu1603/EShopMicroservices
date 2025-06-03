namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(string Name, string Description, List<string> Category, string ImageFile, decimal Price):ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is Required!");
        RuleFor(x => x.Category).NotEmpty().WithMessage("Category is Required!");
        RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is Required!");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price should be greater than 0!");
    }
}
internal class CreateProductCommandHandler(IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        //create a product entity from command object
        var product = new Product
        {
            Name = command.Name,
            Description = command.Description,
            Category = command.Category,
            ImageFile = command.ImageFile,
            Price = command.Price,
        };

        //save to database
        session.Store(product);                              //storing the product in session obj
        await session.SaveChangesAsync(cancellationToken);

        //return the CreateProductResult result
        return new CreateProductResult(product.Id);
    }
}

