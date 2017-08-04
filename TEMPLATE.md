Things to do when creting a project from the template:

1. Open **Visual Studio Code**, or any editor which can see all the files
   in the folder, e.g. **Sublime Text** and **Notepad++**.

Then:

2. Rename the files **project-name-here.sln** and
   **project-name-here.sln.DotSettings**
3. Search and replace all the occurrences of **54321** with the actual port
   used by the web service. All PCS microservices have an assigned port
   number, see the wiki.

Then:

**Non case sensitive** search&replace:

4. "project-name-here"
5. "ProjectNameHere"
6. "PROJECT-ID-HERE"
7. "project name here"

Then, open Visual Studio and:

1. Make sure the project starts pressing F5
2. Update the scripts to check for and use your environment variables
3. Remove this file
4. Remove the example code and write your code
