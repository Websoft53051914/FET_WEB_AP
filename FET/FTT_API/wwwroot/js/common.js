// JavaScript source code
'use strict';

/**
 * 驗證手機格式
 * @param {String} text 手機字串
 * @returns {Boolean} 手機格式是否合法
 */
function ValidateMobilePhone(text) {
    const mobilePhoneRegex = /^09[0-9]{8}$/;
    return mobilePhoneRegex.test(text);
}

/**
 * 驗證Email格式
 * @param {String} text Email字串
 * @returns {Boolean} Email格式是否合法
 */
function ValidateEmail(text) {
    const emailRegex = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$/;
    return emailRegex.test(text);
}

/**
 * 驗證市話格式
 * @param {String} text 市話字串
 * @returns {Boolean} 市話格式是否合法
 */
function ValidateTelephone(text) {
    const telephoneRegex = /^(\d{2,3}-)?\d{6,8}$/;
    return telephoneRegex.test(text);
}

/**
 * 驗證統一編號
 * @param {String} text 統編字串
 * @returns {Boolean} 統編格式是否合法
 */
function ValidateBusinessNO(text) {
    const businessNOLength = 8;
    const divisor = 5;
    if (businessNOLength != 8) {
        return false;
    }
    const multipliers = [1, 2, 1, 2, 1, 2, 4, 1];
    let total = 0;
    let seven = false;
    for (let i = 0; i < businessNOLength; i++) {
        if (i == 6 && text[i] == 7) {
            seven = true;
            continue;
        }
        let product = text[i] * multipliers[i];
        if (product >= 10) {
            const f = Math.floor(product / 10);
            const s = product % 10;
            product = f + s;
        }
        total += product;
    }
    return (total % divisor == 0) || (seven && (((total + 1) % divisor == 0) || (total % divisor == 0)));
}


//js版StringBuilder，當字串要反覆的加上時，相較於單純字串相加，記憶體需求較少
function StringBuilder() {
    this.builder = [];
}

StringBuilder.prototype.Append = function (text) {
    this.builder.push(text);
}

StringBuilder.prototype.AppendLine = function (text) {
    this.builder.push(text + "\n");
}

StringBuilder.prototype.toString = function () {
    return this.builder.join('');
}

Object.defineProperty(StringBuilder.prototype, 'Length', {
    get: function () { return this.builder.length; }
})

/**
 * 時分格式轉換(MMss->MM:ss AM/PM)
 * @param {String} time 時間字串(MMss)
 * @returns {String} 時間字串(Mmss AM/PM)
 */
function ProcessTime(time) {
    let result = '';
    if (!time) {
        return result;
    }
    if (time && /^\d{4}$/.test(time)) {
        var hours = parseInt(time.substring(0, 2));
        var minutes = parseInt(time.substring(2, 4));

        var meridiem = (hours >= 12) ? 'PM' : 'AM';

        hours = (hours > 12) ? hours - 12 : hours;
        var formattedTime = ('0' + hours).slice(-2) + ':' + ('0' + minutes).slice(-2) + ' ' + meridiem;

        result = formattedTime;
    }
    else {
        let error = new Error('時間格式不正確，應為MMss');
        error.name = "ProcessTime(time)";
        throw error;
    }
    return result;

}

/**
 * 判斷是否為空值，null、undefined、空字串皆為空值
 * @param {any} text
 * @returns {Boolean} 是否為空值
 */
function IsNull(text) {
    if (typeof (text) == 'number') {
        return false;
    }
    return !text;
}

/**
 * 數值> 0檢查
 * @param {Number} num
 * @returns {Boolean} 是否大於0
 */
function GreaterThanZero(num) {
    if (typeof (num) != 'number') {
        let error = new Error('輸入的值必須是數字型態');
        error.name = "GreaterThanZero(num)";
        throw error;
    }
    return num > 0;
}


/**
 * 找尋最大值
 * @param {Array<Number>} list
 * @returns {Number} 陣列中最大值
 */
function FindMax(list) {
    let error = new Error('');
    error.name = "FindMax(list)";
    if (!Array.isArray(list)) {
        error.message = "輸入必須是陣列";
    }
    if (Array.isArray(list)&&list.length == 0) {
        error.message = "陣列的長度必須大於0";
    }
    if (error.message) {
        throw error;
    }
    var max = list[0];
    for (let i = 0; i < list.length; i++) {
        if (list[i] > max) {
            max = list[i];
        }
    }
    return max;
}


/**
* 找尋最小值
* @param {Array<Number>} list
* @returns {Number} 陣列中最小值
*/
function FindMin(list) {
    let error = new Error('');
    error.name = "FindMin(list)";
    if (!Array.isArray(list)) {
        error.message = "輸入必須是陣列";
    }
    if (Array.isArray(list) && list.length == 0) {
        error.message = "陣列的長度必須大於0";
    }
    if (error.message) {
        throw error;
    }
    var min = list[0];
    for (let i = 0; i < list.length; i++) {
        if (list[i] < min) {
            min = list[i];
        }
    }
    return min;
}

/**
 * 取得副檔名
 * @param {String} fileName
 * @returns {String} 副檔名
 */
function GetExtension(fileName) {
    fileName = fileName || '';

    return fileName.substring(fileName.lastIndexOf('.'), fileName.length) || fileName;
}

/**
 * Base64 String轉成TypedArray，通常用於下載檔案
 * @param {String} base64 Base64 String
 * @returns TypedArray
 */
function Base64ToTypedArray(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}

/**
 * 產生uuidv4
 * @returns {String} 產生的uuidv4
 */
function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}


/**
 * 在QueryString的value為陣列時，用單純的SearchParams加入會顯得費時，必須要一個一個append進來，此方法可以迅速產出value為陣列的QueryString
 * @param {object} params
 * @returns 
 */
function CreateSearchParams(params) {
    return new URLSearchParams(Object.entries(params).flatMap(([key, values]) => Array.isArray(values) ? values.map((value) => [key, value]) : [[key, values]]));
}


/**
* 針對object of array，用object的key當作Group Key做GroupBy
* @param {Array<Object>} key object of array
* @param {String} key 當作Group Key
* @returns {Object<key,Array<Object>>} Grouping Object
*/
function GroupBy(xs = [], key) {
    return xs.reduce(function (rv, x) {
        (rv[x[key]] = rv[x[key]] || []).push(x);
        return rv;
    }, {});
};

/**
* 小數做四捨五入
* @param {Number} val 要四捨五入的數字
* @param {Number} precision 四捨五入位數
* @returns {Number} 四捨五入後的數字
*/
function RoundDecimal(val, precision) {
    return Math.round(Math.round(val * Math.pow(10, (precision || 0) + 1)) / 10) / Math.pow(10, (precision || 0));
}

/**
 * 取得目前的ScrollBar寬度(px)
 */
function GetScrollbarWidth() {

    // Creating invisible container
    const outer = document.createElement('div');
    outer.style.visibility = 'hidden';
    outer.style.overflow = 'scroll'; // forcing scrollbar to appear
    outer.style.msOverflowStyle = 'scrollbar'; // needed for WinJS apps
    document.body.appendChild(outer);

    // Creating inner element and placing it in the container
    const inner = document.createElement('div');
    outer.appendChild(inner);

    // Calculating difference between container's full width and the child width
    const scrollbarWidth = (outer.offsetWidth - inner.offsetWidth);

    // Removing temporary elements from the DOM
    outer.parentNode.removeChild(outer);

    return scrollbarWidth;

}


function Chr(codePt) {
    if (codePt > 0xFFFF) {
        codePt -= 0x10000;
        return String.fromCharCode(0xD800 + (codePt >> 10), 0xDC00 + (codePt & 0x3FF));
    }
    return String.fromCharCode(codePt);
}

/**
 * Excel的數字轉Column Name
 * @param {Number} number
 * @returns {String} Excel Column Name
 */
function NumberCol2ExcelCol(number) {
    var numeric = (number - 1) % 26;
    var letter = Chr(65 + numeric);
    var number2 = parseInt((number - 1) / 26);
    if (number2 > 0) {
        return NumberCol2ExcelCol(number2) + letter;
    } else {
        return letter;
    }
}

/**
 * Excel的Column Name轉數字(Ex:'A':1,'B':2,'Z':26,'AA':27)
 * @param {String} excelcol Excel的Column
 * @returns {Number} Excel Column數字
 */
function ExcelCol2NumberCol(excelcol) {
    var alphabet2numdict = {};
    for (let i = 0; i < 26; i++) {
        const alphabet = String.fromCharCode(i + 'A'.charCodeAt(0))
        alphabet2numdict[alphabet] = (i + 1);
    }
    var numbercol = 0;
    for (let i = 0; i < excelcol.length; i++) {
        numbercol += alphabet2numdict[excelcol[i]] * Math.pow(26, excelcol.length - i - 1);
    }
    return numbercol;
}



/**
 * SUM公式的Dependencys整理
 * @param {String} formulaContent 
 * @param {Array<Object>} keyCol 
 * @param {Array<String>} dependencys
 * @param {Set} vertexes
 * @returns {void}
 */
function SumDependencysFormula(formulaContent, keyCol, dependencys, vertexes) {
    let fromCol = new StringBuilder();
    let toCol = new StringBuilder();
    let ptr = 4;
    while (formulaContent[ptr] != ':') {
        fromCol.Append(formulaContent[ptr]);
        ptr++;
    }
    ptr++;
    while (formulaContent[ptr] != ')') {
        toCol.Append(formulaContent[ptr]);
        ptr++;
    }


    let fromColNum = excelcol2numbercol(fromCol.toString());
    let toColNum = excelcol2numbercol(toCol.toString());
    if (fromColNum > toColNum) {
        [fromColNum, toColNum] = [toColNum, fromColNum];
    }

    for (let i = fromColNum; i <= toColNum; i++) {
        const colName = numbercol2excelcol(i);
        vertexes.add(colName);
        if (dependencys[keyCol].indexOf(colName) == -1) {
            dependencys[keyCol].push(colName);
        }
    }
}

/**
 * 一般公式的Dependencys整理
 * @param {String} formulaContent
 * @param {Array<Object>} keyCol
 * @param {Array<String>} dependencys
 * @param {Set} vertexes
 * @returns {void}
 */
function DefaultDependencysFormula(formulaContent, keyCol, dependencys, vertexes) {
    let operands = formulaContent.match(/[A-Z]+/g);
    if (operands != null) {
        for (let i = 0; i < operands.length; i++) {
            vertexes.add(operands[i].toUpperCase());
            if (dependencys[keyCol].indexOf(operands[i]) == -1) {
                dependencys[keyCol].push(operands[i]);
            }
        }
    }
}


//檢查循環參照製作依賴清單Strategies Pattern
const dependencysFormulaStrategies = {
    'sum': SumDependencysFormula,
    'default': DefaultDependencysFormula
};

/**
 * Sum公式是否自我參照
 * @param {String} formula 公式內容
 * @param {String} column 自我參照的Column Name
 * @returns {Boolean} 是否自我參照
 */
function IsSelfSumFormula(formula, column) {
    let fromCol = new StringBuilder();
    let toCol = new StringBuilder();
    let ptr = 4;
    while (formula[ptr] != ':') {
        fromCol.Append(formula[ptr]);
        ptr++;
    }
    ptr++;
    while (formula[ptr] != ')') {
        toCol.Append(formula[ptr]);
        ptr++;
    }

    let fromColNum = excelcol2numbercol(fromCol.toString());
    let toColNum = excelcol2numbercol(toCol.toString());
    if (fromColNum > toColNum) {
        [fromColNum, toColNum] = [toColNum, fromColNum];
    }

    const colNum = excelcol2numbercol(column);
    for (let i = fromColNum; i <= toColNum; i++) {
        if (i == colNum) {
            return true;
        }
    }
    return false;
}

/**
 * 一般公式是否自我參照
 * @param {String} formula 公式內容
 * @param {String} column 自我參照的Column Name
 * @returns {Boolean} 是否自我參照
 */
function IsSelfDefaultFormula(formula, column) {
    let operands = formula.match(/[A-Z]+/g);
    if (operands != null) {
        return operands.find(x => x == column);
    }
}

//自我參照Strategy Pattern
const selfFormulaStrategies = {
    'sum': IsSelfSumFormula,
    'default': IsSelfDefaultFormula
};

/**
 * 此公式是否自我參照
 * @param {String} formula 公式內容
 * @param {String} column 自我參照的Column Name
 * @returns {Boolean} 是否自我參照
 */
function IsSelfFormula(formula, column) {
    const issumRegex = /^[S]{1}[U]{1}[M]{1}/
    if (issumRegex.test(formula)) {
        return selfFormulaStrategies['sum'](formula, column);
    }
    return selfFormulaStrategies['default'](formula, column);
}

/**
 * 驗證此vertex有沒有cycle
 * @param {Object} G 要驗證的單向圖
 * @param {String} n 被驗證的vertex
 * @param {Set} path 走訪路徑
 * @param {Set} visited 走訪過的vertex清單
 */
function GetCycle(G, n, path, visited) {
    if (path.has(n)) {
        const v = [...path]
        throw `cycle ${v.slice(v.indexOf(n)).concat(n).join('<-')}`
    }
    visited.add(n)
    path.add(n)
    return G[n].forEach(next => GetCycle(G, next, new Set(path), visited))
}

/**
 * 驗證此張單向圖是否有cycle
 * @param {Object} G 要驗證的單向圖，格式:{p1:[p2,p3,p4],p2:[],p3:[p4],p4:[]}
 */
function Validate(G) {
    const visited = new Set();
    try {
        Object.keys(G).forEach(n => {
            if (visited.has(n)) return
            GetCycle(G, n, new Set(), visited)
        });
        return true;
    }
    catch (ex) {
        return false;
    }
}

/**
* 驗證此公式有無循環參照
* @param {String} thisFormula 被編輯的Column公式內容
* @param {Array<Object>} otherColObjs 除了被編輯的Column以外的公式清單，Ex:{'A':'B+C','D':'E+f'}
* @param {String} alphabetcol 被編輯的Column Name
* @returns {Boolean} 是否有無循環參照
*/
function IsRecursiveReference(thisFormula, otherColObjs, alphabetcol) {
    var vertexes = new Set();
    var dependencys = {};
    const issumRegex = /^[S]{1}[U]{1}[M]{1}/;
    //除了被編輯的Column以外的公式清單整理
    for (const [key, formulaContent] of Object.entries(otherColObjs)) {
        if (!dependencys[key]) {
            dependencys[key] = [];
        }
        vertexes.add(key);
        if (issumRegex.test(formulaContent)) {
            dependencysFormulaStrategies['sum'](formulaContent, key, dependencys, vertexes);
        }
        else {
            dependencysFormulaStrategies['default'](formulaContent, key, dependencys, vertexes);
        }
    }
    //被編輯的Column公式清單整理
    vertexes.add(alphabetcol);
    if (!dependencys[alphabetcol]) {
        dependencys[alphabetcol] = [];
    }
    if (issumRegex.test(thisFormula)) {
        dependencysFormulaStrategies['sum'](thisFormula, alphabetcol, dependencys, vertexes);
    }
    else {
        dependencysFormulaStrategies['default'](thisFormula, alphabetcol, dependencys, vertexes);
    }
    vertexes.forEach(function (item) {
        if (!dependencys[item]) {
            dependencys[item] = [];
        }
    });
    if (IsSelfFormula(thisFormula, alphabetcol)) {
        return true;
    }
    return !Validate(dependencys);
}

/**
 * 西元日期轉民國日期
 * @param {String} adDate 西園日期字串
 * @param {String} dateFormat 日期格式，目前支援yyyy/MM/dd yyy-MM-dd yyyyMMdd
 * @returns {String} 民國日期
 */
function ADDate2MingGuoDate(adDate, dateFormat) {
    let arrDay = [];
    let minguoDate = '';
    switch (dateFormat) {
        case 'yyyy/MM/dd':
            arrDay = adDate.split('/');
            minguoDate = (parseInt(arrDay[0]) - 1911).toString() + "/" + arrDay[1].toString() + "/" + arrDay[2].toString();
            break;
        case 'yyyy-MM-dd':
            arrDay = adDate.split('-');
            minguoDate = (parseInt(arrDay[0]) - 1911).toString() + "-" + arrDay[1].toString() + "-" + arrDay[2].toString();
            break;
        case 'yyyyMMdd':
            let adYear = parseInt(adDate.slice(0, 4));
            let date = adDate.slice(4, 8);
            minguoDate = (adYear - 1911).toString() + date;
            break;
        default:
            break;
    }
    return minguoDate;
}

/**
 * 起訖日期檢查，開始與結束日期的格式必須一致
 * @param {String} fromDate 開始日期字串
 * @param {String} toDate 結束日期字串
 * @returns {Boolean} 起訖日期檢查結果
 */
function ValidateStartEndDate(fromDate, toDate) {
    if (fromDate && toDate) {
        return fromDate <= toDate;
    }
    return true;
}

/**
 * 驗證身分證字號
 * @param {String} id
 * @returns {Boolean} 身分證字號檢查結果
 */
function ValidateId(id) {
    id = id.trim();
    //在 js 中遇到反斜線要跳脫，所以這邊用兩個反斜線
    verification = id.match("^[A-Z][12]\\d{8}$")
    if (!verification) {
        return false
    }

    let conver = "ABCDEFGHJKLMNPQRSTUVXYWZIO"
    let weights = [1, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1]

    id = String(conver.indexOf(id[0]) + 10) + id.slice(1);

    checkSum = 0
    for (let i = 0; i < id.length; i++) {
        c = parseInt(id[i])
        w = weights[i]
        checkSum += c * w
    }

    return checkSum % 10 == 0
}

function thousandedNumber(val) {
    var valstr = val.toString();
    var valsplitarray = valstr.split(".");
    valsplitarray[0] = valsplitarray[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    const thousandedNum = valsplitarray.length == 2 ? valsplitarray[0] + "." + valsplitarray[1] : valsplitarray[0];
    return thousandedNum;
}

/**
 * 倒計時登出 (含Heartbeat) ，layout.cshtml用
 * 需要sweet alert & jquery  
 * @param {number} logoutTime 自動登出分鐘
 * @param {string} checkLoginUrl 檢查登入狀態url
 * @param {string} logoutUrl 登出url
 * @param {HTMLElement} displayTarget 顯示timer taget
 */
function autoLogoutTimer(logoutTime, checkLoginUrl, logoutUrl, displayDom) {
    var idleCounter = 0;
    var idleLimit = logoutTime * 60;

    $("body").on("mousedown keydown", function () {
        idleCounter = 0;
    });

    var hndHeartbeat = setInterval(function () {
        $.post(checkLoginUrl).done(function (res) {
        });
    }, 30 * 1000);

    var hndIdleDetect = setInterval(function () {
        idleCounter++;

        var resultCount = idleLimit - idleCounter;

        const minutes = Math.floor(resultCount / 60);
        const seconds = resultCount % 60;
        const result = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;

        if (displayDom) {
            $(displayDom).html(result + '後登出');
        }

        if (idleCounter > idleLimit) {
            clearInterval(hndHeartbeat);
            clearInterval(hndIdleDetect);

            swal({
                text: '閒置逾時，請重新登入',
                button: '確認',
                icon: 'info'
            }).then((result) => {
                window.location = logoutUrl;
            });
        }
    }, 1000);

}

// 取得本週日期
function getCurrentWeekDates() {
    const monday = moment().startOf('isoWeek'); // 取得本週一
    return Array.from({ length: 7 }, (_, i) => monday.clone().add(i, 'days').format('YYYY-MM-DD'));
}