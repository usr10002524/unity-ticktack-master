/**
 * Created by miguelferreira on 02/09/2017.
 */

var gameInstance = null;
var gameCanvas = null;
var runtimeInitialized = false;
var canvasAspectRatio = false;
var canvasMinWidth = null;
var canvasMaxWidth = null;

function handleResize() {
    if (gameCanvas != null) {
        var canvasSize = getCanvasSize();
        gameCanvas.width = canvasSize.width;
        gameCanvas.height = canvasSize.height;

    }
}

document.addEventListener("DOMContentLoaded", function (event) {
    window.addEventListener("resize", handleResize);
    handleResize();
});


function OnRuntimeIntialized() {
    runtimeInitialized = true;
    gameCanvas = document.querySelector("#unity-canvas");
    gameCanvas.style.width = null;
    gameCanvas.style.height = null;
    handleResize();
}

function getCanvasSize() {
    var windowWidth = window.innerWidth;
    var windowHeight = window.innerHeight;

    if (canvasAspectRatio) {
        var aspectWindowHeight = windowWidth / canvasAspectRatio;
        if (aspectWindowHeight > windowHeight) {
            aspectWindowHeight = windowHeight;
        }

        var aspectWindowWidth = aspectWindowHeight * canvasAspectRatio;
        if (aspectWindowWidth > windowWidth) {
            aspectWindowWidth = windowWidth;
        }

        windowHeight = aspectWindowHeight;
        windowWidth = aspectWindowWidth;

        if (canvasMaxWidth) {
            if (windowWidth > canvasMaxWidth) {
                windowWidth = canvasMaxWidth;
                windowHeight = windowWidth / canvasAspectRatio;
            }
        }
        if (canvasMinWidth) {
            if (windowWidth < canvasMinWidth) {
                windowWidth = canvasMinWidth;
                windowHeight = windowWidth / canvasAspectRatio;

            }
        }
    }

    return { width: windowWidth, height: windowHeight };
}

function getCanvasRatio() {
    var windowWidth = window.innerWidth;
    var windowHeight = window.innerHeight;
    var aspectWindowWidth = windowWidth;
    var aspectWindowHeight = windowHeight;

    if (canvasAspectRatio) {
        aspectWindowHeight = windowWidth / canvasAspectRatio;
        if (aspectWindowHeight > windowHeight) {
            aspectWindowHeight = windowHeight;
        }

        aspectWindowWidth = aspectWindowHeight * canvasAspectRatio;
        if (aspectWindowWidth > windowWidth) {
            aspectWindowWidth = windowWidth;
        }

        if (canvasMaxWidth) {
            if (aspectWindowWidth > canvasMaxWidth) {
                aspectWindowWidth = canvasMaxWidth;
                aspectWindowHeight = aspectWindowWidth / canvasAspectRatio;
            }
        }
        if (canvasMinWidth) {
            if (aspectWindowWidth < canvasMinWidth) {
                aspectWindowWidth = canvasMinWidth;
                aspectWindowHeight = aspectWindowWidth / canvasAspectRatio;
            }
        }
    }

    var windowWidthRatio = aspectWindowWidth / windowWidth;
    var windowHeightRatio = aspectWindowHeight / windowHeight;

    return { width: windowWidthRatio, height: windowHeightRatio };
}

function InitResponsibleWindow(unityInstance, settings) {
    console.log("InitResponsibleWindow called");
    gameInstance = unityInstance;
    if (settings) {
        var aspectRatioComponents = settings.aspectRatio.split(":");
        if (aspectRatioComponents.length != 2) {
            console.exception("Unity: Aspect Ratio tag doesn't follow the expect aspect ratio format A:B e.g. 16:9")
            return;
        }

        canvasAspectRatio = aspectRatioComponents[0] / aspectRatioComponents[1];
        console.log("InitResponsibleWindow aspectRatio:" + canvasAspectRatio);

        if (settings.minWidth) {
            canvasMinWidth = Math.trunc(parseInt(settings.minWidth));
            console.log("InitResponsibleWindow minWidth:" + canvasMinWidth);
        }
        if (settings.maxWidth) {
            canvasMaxWidth = Math.trunc(parseInt(settings.maxWidth));
            console.log("InitResponsibleWindow maxWidth:" + canvasMaxWidth);
        }

        OnRuntimeIntialized();
    }
}

