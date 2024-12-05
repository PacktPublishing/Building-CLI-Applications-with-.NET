
using System.CommandLine;
using bookmarkr.Services;
using Microsoft.Extensions.Options;


namespace bookmarkr.Commands;

public class LinkAddCommand : Command
{

    #region Properties

    private readonly IBookmarkService _service;

    #endregion

    #region Constructor

    public LinkAddCommand(IBookmarkService service, string name, string? description = null)
        : base(name, description)
    {
        _service = service;

        AddOption(nameOption);
        AddOption(urlOption);
        AddOption(categoryOption);

        this.SetHandler(OnHandleAddLinkCommand, nameOption, urlOption, categoryOption);
        
        ConfigureOptions();
    }

    private void ConfigureOptions()
    {
        urlOption.AddValidator(result =>
        {
            foreach (var token in result.Tokens)
            {
                if (string.IsNullOrWhiteSpace(token.Value))
                {
                    result.ErrorMessage = "URL cannot be empty";
                    break;
                }
                else if (!Uri.TryCreate(token.Value, UriKind.Absolute, out _))
                {
                    result.ErrorMessage = $"Invalid URL: {token.Value}";
                    break;
                }
            }
        });

        categoryOption.SetDefaultValue("Read later");
        categoryOption.FromAmong("Read later", "Tech books", "Cooking", "Social media");
        categoryOption.AddCompletions("Read later", "Tech books", "Cooking", "Social media");
    }

    #endregion


    #region Options    
    private Option<string[]> nameOption = new Option<string[]>(
        ["--name", "-n"],
        "The name of the bookmark"
    )
    {
        IsRequired = true,
        Arity = ArgumentArity.OneOrMore,
        AllowMultipleArgumentsPerToken = true
    };

    private Option<string[]> urlOption = new Option<string[]>(
        ["--url", "-u"],
        "The URL of the bookmark"
    )
    {
        IsRequired = true,
        Arity = ArgumentArity.OneOrMore,
        AllowMultipleArgumentsPerToken = true
    };

    private Option<string[]> categoryOption = new Option<string[]>(
        ["--category", "-c"],
        "The category to which the bookmark is associated"
    )
    {
        Arity = ArgumentArity.OneOrMore,
        AllowMultipleArgumentsPerToken = true
    };

    #endregion 

    #region Handler method

    private void OnHandleAddLinkCommand(string[] names, string[] urls, string[] categories)
    {
        _service.AddLinks(names, urls, categories);
        _service.ListAll();
    }

    #endregion
}