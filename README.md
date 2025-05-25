.NET CLI Tool with System.CommandLine

Project Overview

I developed a command-line interface (CLI) application in ‎.NET using the experimental System.CommandLine library. 
This tool allows users to bundle source code files into a single output file, with flexible options for filtering, formatting, and metadata. The application includes two main commands: bundle and create-rsp.

Command-Line Syntax Overview

System.CommandLine parses input by splitting the command string into tokens — typically by whitespace.
Each token may represent a command, option, or argument. The application defines how these tokens are interpreted.

Key Concepts

Commands: Represent actions (e.g., bundle, create-rsp).

Root Command: The executable name of the application.

Subcommands: Nested commands (verbs).

Options: Parameters prefixed with --, may include values (e.g., --output result.txt).

Arguments: Positional values passed to commands or options.

Aliases: Short forms for commands and options (e.g., -o for --output).

Response Files: Files that store command input, referenced with @filename.rsp.

Implemented CLI Commands

🔹 bundle Command

Bundles selected source code files into a single file.

Options:

Option	Alias	Description

--language	-l	Required. Programming languages to include. Use all to include all file types.

--output	-o	Required. Target bundle file name or full path.

--note	-n	Optional. If provided, adds comments with the original file path above each file's content.

--sort	-s	Optional. Sort order: name (default) or type.

--remove-empty-lines	-r	Optional. Removes empty lines from the bundled output.

--author	-a	Optional. Adds an author name comment at the top of the bundle file.

✅ The tool performs validations on input and skips unwanted directories such as bin, obj, debug, etc.

Example:

dotnet mycli bundle --language cs --output "bundle.txt" --note --sort name --remove-empty-lines --author "Jane Doe"
🔹 create-rsp Command
Simplifies repeated command execution by generating a response file.

When the user runs create-rsp, the CLI prompts for input for each bundle option.
It then generates a .rsp file containing the full command line with the provided values.

Example usage:

dotnet mycli create-rsp

The user is guided through a series of prompts.

The resulting file (e.g., bundle.rsp) can be executed as:


dotnet mycli @bundle.rsp

Development Notes

Aliases were defined for all options to simplify typing.

Validation is performed for required options, file paths, and value constraints.

Modular Design: Each command is implemented as a separate handler with custom logic.

Cross-platform Compatibility: Works on Windows, Linux, and macOS.

Running the CLI Globally

To run the CLI from any terminal location:

Build and publish the application:

dotnet publish -c Release -o ./publish

Add the publish folder to the system PATH:

Open System Properties → Environment Variables

Edit the Path variable and add the full path to the publish folder.

Run from anywhere:

mycli bundle --language cs --output result.txt
