const fs = require('fs')
const path = require('path')
const { Octokit } = require('@octokit/rest')

const octokit = new Octokit({
  auth: process.env.GITHUB_TOKEN
})

const repoDetails = process.env.GITHUB_REPOSITORY.split('/')

const owner = repoDetails[0]
const repo = repoDetails[1]

async function issueExists (title) {
  const { data: issues } = await octokit.request('GET /repos/{owner}/{repo}/issues', {
    owner: 'PSButlerII',
    repo: 'MyFreeFormForm',
    state: 'open',
    labels: 'todo',
    // headers: {
    //   'X-GitHub-Api-Version': '2022-11-28'
    // }
  })

  return issues.some(issue => issue.title === title)
}

const sleep = ms => new Promise(resolve => setTimeout(resolve, ms));

async function createIssue(title, body) {
  try {
    const exists = await issueExists(title);
    if (!exists) {
      await octokit.request('POST /repos/{owner}/{repo}/issues', {
        owner,
        repo,
        title,
        body,
        labels: ['todo']
      });
      console.log(`Created issue: ${title}`);
    } else {
      console.log(`Issue already exists: ${title}`);
    }
  } catch (error) {
    if (error.status === 403 && error.response.headers['x-ratelimit-remaining'] === '0') {
      // Extract rate limit reset time from the headers
      const resetTime = parseInt(error.response.headers['x-ratelimit-reset']) * 1000; // Convert to milliseconds
      const now = Date.now();
      const waitTime = resetTime - now + 1000; // Add a 1 second buffer
      console.log(`Rate limit exceeded. Waiting ${waitTime / 1000} seconds before retrying.`);
      await sleep(waitTime); // Wait until the rate limit is reset
      return createIssue(title, body); // Retry creating the issue
    } else {
      throw error; // Rethrow the error if it's not a rate limit issue
    }
  }
}


function findTODOs(filePath) {
  const content = fs.readFileSync(filePath, 'utf8');
  const lines = content.split('\n');

  lines.forEach((line, index) => {
      // Use a regular expression to find 'TODO' more flexibly
      const match = line.match(/TODO:/i); // Case-insensitive match for 'TODO:'
      if (match) {
          // Ensure we actually have text after 'TODO:'
          const splitIndex = match.index + match[0].length;
          const todoText = line.substring(splitIndex).trim();
          if (todoText) { // Check that todoText is not empty
              const title = `TODO: ${todoText}`;
              const body = `Found a TODO comment in [${filePath}] at line ${index + 1}: ${todoText}`;
              console.log(`Body of issue: ${body}`)
              createIssue(title, body).catch(console.error);
          }
      }
  })
}


function scanDirectory(directory, ignoreDirs = ['node_modules','dist','build','lib','coverage','bin','obj','.git','Migrations']) {
  fs.readdirSync(directory, { withFileTypes: true }).forEach(dirent => {
    const fullPath = path.join(directory, dirent.name);
    console.log(`Visiting directory: ${directory}`);
    if (dirent.isDirectory()) {
      // Skip the directory if it's in the ignore list
      console.log(`Entering directory: ${fullPath}`);
      if (ignoreDirs.includes(dirent.name)) {
        console.log(`Skipping directory: ${fullPath}`);
        return; // Skip this directory
      }
      scanDirectory(fullPath, ignoreDirs); // Recursively scan subdirectories
    } else if (dirent.isFile()){
      console.log(`Found file: ${fullPath}`);
      const fileExtensions = ['.js', '.cs', '.json', '.cshtml'];
      const fileExtension = path.extname(dirent.name);
      if (fileExtensions.includes(fileExtension)) {
        findTODOs(fullPath); // Process the file if its extension matches
      }
    }
  });
}

console.log(`Scanning starts from: ${process.env.GITHUB_WORKSPACE}`);
scanDirectory(process.env.GITHUB_WORKSPACE)
