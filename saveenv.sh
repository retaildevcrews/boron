#!/bin/bash

export He_Repo=boron
export AUTH_TYPE=CLI

if [ -z "$He_Name" ]
then
  echo "Please set He_Name before running this script"
else
  if [ -f ~/.boron.env ]
  then
    if [ "$#" = 0 ] || [ $1 != "-y" ]
    then
      read -p "boron.env already exists. Do you want to remove? (y/n) " response

      if ! [[ $response =~ [yY] ]]
      then
        echo "Please move or delete ~/.boron.env and rerun the script."
        exit 1;
      fi
    fi
  fi

  export KEYVAULT_NAME=$He_Name

  echo '#!/bin/bash' > ~/.boron.env
  echo '' >> ~/.boron.env

  IFS=$'\n'

  for var in $(env | grep -E 'He_|MI_|AKS_|Imdb_|AUTH_TYPE|KEYVAULT_NAME' | sort | sed "s/=/='/g")
  do
    echo "export ${var}'" >> ~/.boron.env
  done

  cat ~/.boron.env
fi
