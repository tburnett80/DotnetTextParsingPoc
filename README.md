# DotnetTextParsingPoc
Cross platform Text Parsing Proof of Concept

To Run the example, start by building the dockerfile, enter the following command from a console with the present directory set to the Solution root directory.


docker build --tag temp . 


That will build the image, after building the source code.
Now to run said example, run the following command:


docker run -it --name temp temp 


This command will run the console app driver, and it will parse the 3 example files and output the content meta. 
