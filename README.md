# DotnetTextParsingPoc
Cross platform Text Parsing Proof of Concept

This example assumes you have an environment with 2 things:
1. Git installed
2. Docker installed and root / admin access to issue docker commands such as build and running containers. 



To Run the example, start by building the dockerfile, enter the following command from a console with the present directory set to the Solution root directory.


docker build --tag temp . 


That will build the image, after building the source code.
Now to run said example, run the following command:


docker run -it --name temp temp 


This command will run the console app driver, and it will parse the 3 example files and output the content meta. 
