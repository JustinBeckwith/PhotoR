var child_process = require('child_process'),
	util = require('util'),
	request = require('request'),
	wrench = require('wrench'),
	async = require('async');

/*
 * build settings
 */
var projectName = "PhotoR";
var project = "PhotoR\\PhotoR.csproj";
var config = 'Release';
var deploymentTarget = process.env.DEPLOYMENT_TARGET;
var deploymentTemp = process.env.DEPLOYMENT_TEMP;


/*
 * run the build command, and wait for the callback to exit
 */
async.series([build, minify, complete]);


/*
 * execute MS Build 
 */
function build(callback) {
	console.log('building %s', project);
	var buildCommand = util.format("%WINDIR%\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild %s /v:minimal /t:pipelinePreDeployCopyAllFilesToOneFolder /p:Configuration=%s /p:SolutionDir=%s /p:_PackageTempDir=\"%s\" /p:AutoParameterizationWebConfigConnectionStrings=false", project, config, process.env.DEPLOYMENT_SOURCE, process.env.DEPLOYMENT_TEMP);
	console.log('build command %s', buildCommand);
	var msbuild = child_process.exec(buildCommand, function(error, stdout, stderr) {
		if(error) {
			console.log(error.stack);
			console.log('Error code: ' + error.code);
			console.log('Signal received: ' + error.signal);

			sendMessage(util.format('deploying %s to Windows Azure has failed', projectName));
			process.exit(1);
		}
		if(stderr) console.error(stderr);
		if(stdout) console.log(stdout);	
	});

	msbuild.on('exit', function(code) {
		console.log('MSBuild exited with exit code ' + code);
		callback();
	});	
}

/*
 * minify JavaScript
 */
function minify(callback) {
	console.log('minifying javascript');
	callback();
}

/*
 * copy files from the temp directory to the target directory
 */
function copy(callback) {
	console.log('Copying files from %s to %s', deploymentTemp, deploymentTarget);
	wrench.copyDirSyncRecursive(deploymentTemp, deploymentTarget);
	callback();
}

/*
 * send a success text via the twilio api
 */
function complete(callback) {
	console.log('sending notification text message');
	sendMessage(util.format('deploying %s to Windows Azure was a success!', projectName), function() {
		callback();
	});	
}


/*
 * send a text using the twilio api
 */
function sendMessage(message, callback) {
	request.post({
		headers: {
			'content-type': 'application/x-www-form-urlencoded'
		},
		url: 'https://AC4535a3259069b70dbc641954a9b7ae0f:f5c3c5f80a251cdf571812483d09ba3c@api.twilio.com/2010-04-01/Accounts/AC4535a3259069b70dbc641954a9b7ae0f/SMS/Messages.json',
		body: util.format("From=+4155992671&To=+7247771008&Body=%s", encodeURIComponent(message))
	}, function(error, response, body) {
		if (error) console.log(util.inspect(error));		
		console.log('Twilio request complete: ' + body);
		callback();
	});
}

