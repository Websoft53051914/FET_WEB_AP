var Draw = function (canvasEl, color, drawCallBackEvent = () => { }) {
    this.type = 'pen';
    this.penal = canvasEl;
    this.pen = this.penal.getContext('2d');
    this.isDraw = false;
    this.lineWidth = 5;
    this.color = color;
    this.drawCallBackEvent = drawCallBackEvent;
};

Draw.prototype.init = function () {
    var self = this;

    const getMousePos = (canvas, evt)=> {
        var rect = canvas.getBoundingClientRect();
        return {
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        };
    }
    this.penal.addEventListener('mousedown', function (event) {
        self.isDraw = true;
        self.drawCallBackEvent();
        let mousePos = getMousePos(self.penal, event)
        self.pen.beginPath();
        self.pen.lineCap = "round";
        self.pen.lineJoin = "round";
        self.pen.shadowBlur = 1; // 邊緣模糊，防止直線邊緣出現鋸齒
        self.pen.shadowColor = 'black';// 邊緣顏色
        self.pen.moveTo(mousePos.x, mousePos.y);

    }, false);
    this.penal.addEventListener('mousemove', function (event) {
        if (self.isDraw) {

            if (self.type == 'pen') {
                let mousePos = getMousePos(self.penal, event);
                self.pen.lineTo(mousePos.x, mousePos.y);
                self.pen.stroke();
            } else if (self.type == 'robber') {
                self.pen.strokeStyle = '#ccc';
                self.pen.clearRect(x - 10, y - 10, 20, 20);

            }
        }
    }, false);
    this.penal.addEventListener('mouseleave', function () {
        if (self.isDraw) {
            self.isDraw = false;
            self.pen.closePath();
        }
    }, false);
    this.penal.addEventListener('mouseup', function (event) {
        self.isDraw = false;
    }, false);
};

Draw.prototype.toImage = function (mimeType = 'image/png') {
   return  this.penal.toDataURL(mimeType);
}

Draw.prototype.loadImage = function (base64Src, onLoadCallBack = () => { }) {
    var self = this;
    let base_image = new Image();
    base_image.onload = function () {
        self.pen.drawImage(base_image, 0, 0);
        onLoadCallBack();
    }
    base_image.src = base64Src;
}

Draw.prototype.initMobile = function () {
    var self = this;

    const getMousePos = (canvas, evt) => {
        var rect = canvas.getBoundingClientRect();
        return {
            x: (evt.clientX || evt.touches[0].clientX) - rect.left,
            y: (evt.clientY || evt.touches[0].clientY) - rect.top
        };
    }

    // Touch start or mouse down event
    this.penal.addEventListener('mousedown', startDrawing, false);
    this.penal.addEventListener('touchstart', startDrawing, false);

    function startDrawing(event) {
        event.preventDefault(); // 防止滾動或其他行為
        self.isDraw = true;
        self.drawCallBackEvent();

        let mousePos = getMousePos(self.penal, event);
        self.pen.beginPath();
        self.pen.lineCap = "round";
        self.pen.lineJoin = "round";
        self.pen.shadowBlur = 1;
        self.pen.shadowColor = 'black';
        self.pen.moveTo(mousePos.x, mousePos.y);
    }

    // Touch move or mouse move event
    this.penal.addEventListener('mousemove', draw, false);
    this.penal.addEventListener('touchmove', draw, false);

    function draw(event) {
        event.preventDefault(); // 防止滾動或其他行為
        if (self.isDraw) {
            let mousePos = getMousePos(self.penal, event);
            if (self.type == 'pen') {
                self.pen.lineTo(mousePos.x, mousePos.y);
                self.pen.stroke();
            } else if (self.type == 'robber') {
                self.pen.strokeStyle = '#ccc';
                self.pen.clearRect(mousePos.x - 10, mousePos.y - 10, 20, 20);
            }
        }
    }

    // Touch end or mouse up event
    this.penal.addEventListener('mouseup', stopDrawing, false);
    this.penal.addEventListener('mouseleave', stopDrawing, false);
    this.penal.addEventListener('touchend', stopDrawing, false);

    function stopDrawing(event) {
        self.isDraw = false;
        self.pen.closePath();
    }
};

Draw.prototype.clear = function () {
    this.pen.clearRect(0, 0, this.penal.width, this.penal.height);
}