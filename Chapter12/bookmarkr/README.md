
# Bookmarkr
Bookmarkr is a bookmark manager provided as a CLI application.
It has been developed in .NET as an example for the book "Building CLI applications with .NET" by Tidjani Belmansour, published by Packt Publishing.  

# Getting started
Bookmarkr is provided as a .NET Tool and can be installed using this command:
```
dotnet tool install --global bookrmarkr
```
Once installed, it can be run by invoking:
```
bookmarkr
```

# Commands
Here are some examples of the available commands:

```
* dotnet run => "Hello from the root command!"
* dotnet run hello => "Unrecognized command or argument 'hello'."
* dotnet run -- --version => 2.0.0
* dotnet run -- --help | dotnet run -- -h | dotnet run -- -? => help menu for the CLI application.
* dotnet run -- link --help | dotnet run -- link -h | dotnet run -- link -? => help menu for the link command.
* dotnet run -- link add --help | dotnet run -- link add -h | dotnet run -- link add -? => help menu for the link command.
* dotnet run link add --name 'Packt Publishing' --url 'https://packtpub.com/' --category 'Tech books' => "Bookmark successfully added!"
* dotnet run link add -n 'Packt Publishing' -u 'https://www.packtpub.com' -c 'Tech books' => "Bookmark successfully added!"
* dotnet run link add --name 'Packt Publishing' --url 'https://packtpub.com/' --name 'A great tech book publisher' => the second name will override the first name.
* dotnet run link add --name 'Packt Publishing' --url 'https://packtpub.com/' --category 'Tech books' --name 'Audi cars' --url 'https://audi.ca' --category 'Read later' => adding two bookmarks with a single CLI request.
* dotnet run link add --name 'Packt Publishing' 'Audi cars' --url 'https://packtpub.com/' 'https://audi.ca' --category 'Tech books' 'Read later' => an equivalent syntax.
* dotnet run export --file 'bookmarks.json' => exports all the bookmarks held by the application into the specified output JSON file.
* dotnet run import --file 'bookmarks.json' => imports all the bookmarks found in the input JSON file into the application.
* dotnet run -- interactive => runs the interactive version of the application.
* dotnet run sync => calls the web service to sync the local bookmarks with the ones stored in the remote location. 
```