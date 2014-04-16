//pull in our parsers
var mnistParser = require("../lib/mnist-parser.js");
var path = require('path');
var fs = require('fs');

//get absolute path on system
var absDataPath = path.resolve(__dirname, "../data/t10k-images-idx3-ubyte.gz");
var absLabelPath = path.resolve(__dirname, "../data/t10k-labels-idx1-ubyte.gz");

//send for processing in parser
var parser = new mnistParser();

function pullInt32(data, start)
{
	start = start || 0;
	//parse int 
	return ((data[start + 3] & 0xFF) << 0) |
	 ((data[start + 2] & 0xFF) << 8) |
	  ((data[start + 1] & 0xFF) << 16) |
	   ((data[start] & 0xFF) << 24);
}

//grab the meta info here 
parser.on('parseMeta', function(itemCount, width, height)
{
	console.log(" item count: ", itemCount, ", pixelWidth: ", width, ", pixelHeight: ", height);
});

parser.on('parseDigitChunk', function(chunkIx, chunkData, totalChunkCount)
{
	
	var save = JSON.stringify(chunkData);
	fs.writeFileSync(path.resolve(__dirname, '../build/chunks/' + chunkIx + ".json"), save);

	console.log("chunk-" + (chunkIx + 1) + " out of " + totalChunkCount + " images in total");
});

parser.on('parseError', function(e)
{
	console.log("Parse error: ", e);
});


parser.parseMNIST(absLabelPath, absDataPath, 20);