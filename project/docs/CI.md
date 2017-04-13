Continuous Integration
======================

CI runs via [Travis CI](https://travis-ci.org) for each pull request and each code change.

### Steps to setup a new project

Create a `.travis.yml` in the root of the GitHub project. You can copy the file from the 
template. The file looks like this:

```
language: csharp
mono: none
dotnet: 1.0.1
dist: trusty
solution: SOLUTION-FILENAME.sln
script:
- ./project/scripts/build.sh
notifications:
  slack:
    rooms:
    - secure: ENCRYPTED-KEY
```

Go to https://travis-ci.org and click "Add new repository" on the left "+". 
For private repositories use https://travis-ci.com instead. Please note that we have a 
very small subscription for private repos, so we should use only public ones.

Enable CI on the desired repository.

In Travis, go to the build settings:
  * Enable "build only if .travis.yml is present"
  * Enable "limit concurrent jobs" and set the value to 1
  * Enable "build branch updates"
  * Enable "build pull request updates"

### Enable Slack notifications

Slack already has an active token to accept notifications and forward them to 
\#pcs-notifications Slack channel.

To activate Travis-Slack integration, run the following command from the root of the 
project and check in `.travis.yml`:

```
travis login --auto
travis encrypt "azureiot:...key...#pcs-notifications --add notifications.slack"
```

Note that the integration key is encrypted, to avoid external spam. 
The non-encrypted tokens can be found in Slack, starting the channel integrations.

### Microsoft Teams notifications

Microsoft Teams doesn't support encrypted keys, so we cannot enable notifications 
on public repositories.
