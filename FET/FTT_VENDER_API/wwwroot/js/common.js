// JavaScript source code
'use strict';

/**
 * ���Ҥ���榡
 * @param {String} text ����r��
 * @returns {Boolean} ����榡�O�_�X�k
 */
function ValidateMobilePhone(text) {
    const mobilePhoneRegex = /^09[0-9]{8}$/;
    return mobilePhoneRegex.test(text);
}

/**
 * ����Email�榡
 * @param {String} text Email�r��
 * @returns {Boolean} Email�榡�O�_�X�k
 */
function ValidateEmail(text) {
    const emailRegex = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$/;
    return emailRegex.test(text);
}

/**
 * ���ҥ��ܮ榡
 * @param {String} text ���ܦr��
 * @returns {Boolean} ���ܮ榡�O�_�X�k
 */
function ValidateTelephone(text) {
    const telephoneRegex = /^(\d{2,3}-)?\d{6,8}$/;
    return telephoneRegex.test(text);
}

/**
 * ���ҲΤ@�s��
 * @param {String} text �νs�r��
 * @returns {Boolean} �νs�榡�O�_�X�k
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


//js��StringBuilder�A��r��n���Ъ��[�W�ɡA�۸����¦r��ۥ[�A�O����ݨD����
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
 * �ɤ��榡�ഫ(MMss->MM:ss AM/PM)
 * @param {String} time �ɶ��r��(MMss)
 * @returns {String} �ɶ��r��(Mmss AM/PM)
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
        let error = new Error('�ɶ��榡�����T�A����MMss');
        error.name = "ProcessTime(time)";
        throw error;
    }
    return result;

}

/**
 * �P�_�O�_���ŭȡAnull�Bundefined�B�Ŧr��Ҭ��ŭ�
 * @param {any} text
 * @returns {Boolean} �O�_���ŭ�
 */
function IsNull(text) {
    if (typeof (text) == 'number') {
        return false;
    }
    return !text;
}

/**
 * �ƭ�> 0�ˬd
 * @param {Number} num
 * @returns {Boolean} �O�_�j��0
 */
function GreaterThanZero(num) {
    if (typeof (num) != 'number') {
        let error = new Error('��J���ȥ����O�Ʀr���A');
        error.name = "GreaterThanZero(num)";
        throw error;
    }
    return num > 0;
}


/**
 * ��M�̤j��
 * @param {Array<Number>} list
 * @returns {Number} �}�C���̤j��
 */
function FindMax(list) {
    let error = new Error('');
    error.name = "FindMax(list)";
    if (!Array.isArray(list)) {
        error.message = "��J�����O�}�C";
    }
    if (Array.isArray(list)&&list.length == 0) {
        error.message = "�}�C�����ץ����j��0";
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
* ��M�̤p��
* @param {Array<Number>} list
* @returns {Number} �}�C���̤p��
*/
function FindMin(list) {
    let error = new Error('');
    error.name = "FindMin(list)";
    if (!Array.isArray(list)) {
        error.message = "��J�����O�}�C";
    }
    if (Array.isArray(list) && list.length == 0) {
        error.message = "�}�C�����ץ����j��0";
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
 * ���o���ɦW
 * @param {String} fileName
 * @returns {String} ���ɦW
 */
function GetExtension(fileName) {
    fileName = fileName || '';

    return fileName.substring(fileName.lastIndexOf('.'), fileName.length) || fileName;
}

/**
 * Base64 String�নTypedArray�A�q�`�Ω�U���ɮ�
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
 * ����uuidv4
 * @returns {String} ���ͪ�uuidv4
 */
function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}


/**
 * �bQueryString��value���}�C�ɡA�γ�ª�SearchParams�[�J�|��o�O�ɡA�����n�@�Ӥ@��append�i�ӡA����k�i�H���t���Xvalue���}�C��QueryString
 * @param {object} params
 * @returns 
 */
function CreateSearchParams(params) {
    return new URLSearchParams(Object.entries(params).flatMap(([key, values]) => Array.isArray(values) ? values.map((value) => [key, value]) : [[key, values]]));
}


/**
* �w��object of array�A��object��key��@Group Key��GroupBy
* @param {Array<Object>} key object of array
* @param {String} key ��@Group Key
* @returns {Object<key,Array<Object>>} Grouping Object
*/
function GroupBy(xs = [], key) {
    return xs.reduce(function (rv, x) {
        (rv[x[key]] = rv[x[key]] || []).push(x);
        return rv;
    }, {});
};

/**
* �p�ư��|�ˤ��J
* @param {Number} val �n�|�ˤ��J���Ʀr
* @param {Number} precision �|�ˤ��J���
* @returns {Number} �|�ˤ��J�᪺�Ʀr
*/
function RoundDecimal(val, precision) {
    return Math.round(Math.round(val * Math.pow(10, (precision || 0) + 1)) / 10) / Math.pow(10, (precision || 0));
}

/**
 * ���o�ثe��ScrollBar�e��(px)
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
 * Excel���Ʀr��Column Name
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
 * Excel��Column Name��Ʀr(Ex:'A':1,'B':2,'Z':26,'AA':27)
 * @param {String} excelcol Excel��Column
 * @returns {Number} Excel Column�Ʀr
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
 * SUM������Dependencys��z
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
 * �@�뤽����Dependencys��z
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


//�ˬd�`���ѷӻs�@�̿�M��Strategies Pattern
const dependencysFormulaStrategies = {
    'sum': SumDependencysFormula,
    'default': DefaultDependencysFormula
};

/**
 * Sum�����O�_�ۧڰѷ�
 * @param {String} formula �������e
 * @param {String} column �ۧڰѷӪ�Column Name
 * @returns {Boolean} �O�_�ۧڰѷ�
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
 * �@�뤽���O�_�ۧڰѷ�
 * @param {String} formula �������e
 * @param {String} column �ۧڰѷӪ�Column Name
 * @returns {Boolean} �O�_�ۧڰѷ�
 */
function IsSelfDefaultFormula(formula, column) {
    let operands = formula.match(/[A-Z]+/g);
    if (operands != null) {
        return operands.find(x => x == column);
    }
}

//�ۧڰѷ�Strategy Pattern
const selfFormulaStrategies = {
    'sum': IsSelfSumFormula,
    'default': IsSelfDefaultFormula
};

/**
 * �������O�_�ۧڰѷ�
 * @param {String} formula �������e
 * @param {String} column �ۧڰѷӪ�Column Name
 * @returns {Boolean} �O�_�ۧڰѷ�
 */
function IsSelfFormula(formula, column) {
    const issumRegex = /^[S]{1}[U]{1}[M]{1}/
    if (issumRegex.test(formula)) {
        return selfFormulaStrategies['sum'](formula, column);
    }
    return selfFormulaStrategies['default'](formula, column);
}

/**
 * ���Ҧ�vertex���S��cycle
 * @param {Object} G �n���Ҫ���V��
 * @param {String} n �Q���Ҫ�vertex
 * @param {Set} path ���X���|
 * @param {Set} visited ���X�L��vertex�M��
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
 * ���Ҧ��i��V�ϬO�_��cycle
 * @param {Object} G �n���Ҫ���V�ϡA�榡:{p1:[p2,p3,p4],p2:[],p3:[p4],p4:[]}
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
* ���Ҧ��������L�`���ѷ�
* @param {String} thisFormula �Q�s�誺Column�������e
* @param {Array<Object>} otherColObjs ���F�Q�s�誺Column�H�~�������M��AEx:{'A':'B+C','D':'E+f'}
* @param {String} alphabetcol �Q�s�誺Column Name
* @returns {Boolean} �O�_���L�`���ѷ�
*/
function IsRecursiveReference(thisFormula, otherColObjs, alphabetcol) {
    var vertexes = new Set();
    var dependencys = {};
    const issumRegex = /^[S]{1}[U]{1}[M]{1}/;
    //���F�Q�s�誺Column�H�~�������M���z
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
    //�Q�s�誺Column�����M���z
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
 * �褸����������
 * @param {String} adDate ������r��
 * @param {String} dateFormat ����榡�A�ثe�䴩yyyy/MM/dd yyy-MM-dd yyyyMMdd
 * @returns {String} ������
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
 * �_�W����ˬd�A�}�l�P����������榡�����@�P
 * @param {String} fromDate �}�l����r��
 * @param {String} toDate ��������r��
 * @returns {Boolean} �_�W����ˬd���G
 */
function ValidateStartEndDate(fromDate, toDate) {
    if (fromDate && toDate) {
        return fromDate <= toDate;
    }
    return true;
}

/**
 * ���Ҩ����Ҧr��
 * @param {String} id
 * @returns {Boolean} �����Ҧr���ˬd���G
 */
function ValidateId(id) {
    id = id.trim();
    //�b js ���J��ϱ׽u�n����A�ҥH�o��Ψ�Ӥϱ׽u
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
 * �˭p�ɵn�X (�tHeartbeat) �Alayout.cshtml��
 * �ݭnsweet alert & jquery  
 * @param {number} logoutTime �۰ʵn�X����
 * @param {string} checkLoginUrl �ˬd�n�J���Aurl
 * @param {string} logoutUrl �n�Xurl
 * @param {HTMLElement} displayTarget ���timer taget
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
            $(displayDom).html(result + '��n�X');
        }

        if (idleCounter > idleLimit) {
            clearInterval(hndHeartbeat);
            clearInterval(hndIdleDetect);

            swal({
                text: '���m�O�ɡA�Э��s�n�J',
                button: '�T�{',
                icon: 'info'
            }).then((result) => {
                window.location = logoutUrl;
            });
        }
    }, 1000);

}

// ���o���g���
function getCurrentWeekDates() {
    const monday = moment().startOf('isoWeek'); // ���o���g�@
    return Array.from({ length: 7 }, (_, i) => monday.clone().add(i, 'days').format('YYYY-MM-DD'));
}