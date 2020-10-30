#!/bin/sh

# copy vscode files
mkdir -p .vscode
cp .devcontainer/vscode-template/* .vscode

# run dotnet restore
dotnet restore src/boron.sln
dotnet restore src/tests.sln

# set auth type
export PATH="$PATH:$HOME/.dotnet:$HOME/.dotnet/tools"
export KEYVAULT_NAME=boron
export AUTH_TYPE=CLI

# install WebV global tool
dotnet tool install -g webvalidate

# update .bashrc
echo "" >> ~/.bashrc
echo 'export PATH="$PATH:$HOME/.dotnet:$HOME/.dotnet/tools"' >> ~/.bashrc
echo "export KEYVAULT_NAME=boron" >> ~/.bashrc
echo "export AUTH_TYPE=CLI" >> ~/.bashrc

sudo apt-get update
sudo apt-get install -y httpie
