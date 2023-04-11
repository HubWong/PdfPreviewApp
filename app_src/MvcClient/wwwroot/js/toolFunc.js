class ToolFunc {
  constructor (viewDomId) {
    this.domId = viewDomId
  }

  static serialData = data => {
    return Object.keys(data)
      .map(k => `${esc(k)}=${esc(data[k])}`)
      .join('&')
  }

  static endWith = (str, subStr) => {
    let reg = new RegExp(subStr + '$')
    return reg.test(str.toLowerCase())
  }

  //lots of funcs ommited.
  static getUrlParam = function (name) {
    let reg = new RegExp('(^|&)' + name + '=([^&]*)(&|$)')

    let r = String(window.location.search)
      .substring(1)
      .match(reg)

    if (r !== null) return decodeURI(r[2])
    return null
  }

  static checkAll = function (ele) {
    let t = $(ele + ' thead')
      .find('[type=checkbox]')
      .is(':checked')

    if (t) {
      $(ele + ' tbody')
        .find('input[type=checkbox]')
        .prop('checked', true)
    } else {
      $(ele + ' tbody')
        .find('input[type=checkbox]')
        .prop('checked', false)
    }
  }

  static isUrl (str) {
    if(!String(str).startsWith('http') && !String(str).startsWith('https')){
      return false
    }
    let v = new RegExp(
      '^(?!mailto:)(?:(?:http|https|ftp)://|//)(?:\\S+(?::\\S*)?@)?(?:(?:(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}(?:\\.(?:[0-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))|(?:(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)(?:\\.(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)*(?:\\.(?:[a-z\\u00a1-\\uffff]{2,})))|localhost)(?::\\d{2,5})?(?:(/|\\?|#)[^\\s]*)?$',
      'i'
    )
    return v.test(str)
  }

  static add_cls_tr_when_click = (eleTbl, withCls) => {
    $(eleTbl + ' tbody tr')
      .find('input[type="checkbox"]')
      .on('click', function () {
        if ($(this).is(':checked')) {
          $(this)
            .parents('tr')
            .addClass(withCls)
        } else {
          $(this)
            .parents('tr')
            .removeClass(withCls)
        }
      })
  }
  static no_tr = (eleTbl, td_len) => {
    let trs = $(eleTbl + ' tbody')
      .eq(0)
      .find('tr')

    if (trs.length === 0) {
      let tr = document.createElement('tr'),
        td = document.createElement('td')
      td.innerHTML = '没有数据'
      td.setAttribute('colspan', td_len)
      td.setAttribute('style', 'text-align:center')
      $(tr).append(td)
      $(eleTbl + ' tbody')
        .eq(0)
        .append(tr)
    }
  }
  /**
   * rmv tr with certain class,if it's bn emptied, show no data.
   * @param {table element or table class} eleTbl
   * @param {row class} withCls
   */
  static remove_cls_tr = function (eleTbl, withCls) {
    let td_len = $(eleTbl + ' tbody tr:first').find('td').length
    $(eleTbl + ' tbody tr.' + withCls)
      .hide('slow')
      .remove()
    ToolFunc.no_tr(eleTbl, td_len)
  }

  static remove_ckd_tr = function (eleTbl, curCls_row) {
    if (curCls_row) {
      ToolFunc.remove_cls_tr(eleTbl, curCls_row)
    } else {
      let td_len = $(eleTbl + ' tbody tr:first').find('td').length,
        trs = $(eleTbl + ' tbody tr').find('input[type="checkbox"]')
      $(trs).each((a, b) => {
        if ($(b).is(':checked')) {
          $(b)
            .parents('tr')
            .hide('slow')
            .remove()
        }
      })
      ToolFunc.no_tr(eleTbl, td_len)
    }
  }

  /**
   *
   * @param {*} array
   */
  arrayTableRow = function (array) {
    if (array && array.length > 0) {
      for (var i = 0; i < array.length; i++) {
        $('input:checkbox').each(function () {
          if ($(this).val() === array[i]) {
            $(this)
              .parents('tr')
              .hide(2000)
          }
        })
      }
    } else {
      console.log('[*error*]:not defined params')
    }
  }

  requestData = (qs, reqCb) => {
    if (reqCb) {
      reqCb(qs)
    }
  }

  static doAjaxFn = function (url, data, cb, postOrGet) {
    if (postOrGet === 'post') {
      $.post(url, data).done(function (v) {
        if (cb) {
          cb(v)
        }
      })
    } else if (postOrGet === 'get') {
      $.get(url, data).done(function (v) {
        if (cb) {
          cb(v)
        }
      })
    } else if (postOrGet === 'delete') {
      $.ajax({
        url: url + '/' + data,
        asyn: true,
        type: postOrGet,
        success: function (v) {
          if (cb) {
            cb(v)
          }
        },
        error: er => {
          console.log(er)
        }
      })
    }
  }

  static nodeToString = function (node) {
    //html elements to string.
    var tmpNode = document.createElement('div')
    tmpNode.appendChild(node.cloneNode(true))
    var str = tmpNode.innerHTML
    tmpNode = node = null
    return str
  }

  showModal = (modalId, msg) => {
    let node = document.getElementById(modalId),
      md = new bootstrap.Modal(node, {
        keyboard: true
      })
    $(node)
      .find('[class="modal-body"]')
      .text(msg)
    md.toggle()
  }

  showTips = function (msg) {
    $('.tips')
      .html(msg)
      .slideDown('fast')
    var tim = function () {
      $('.tips')
        .slideUp('fast')
        .empty()
    }
    setTimeout(tim, 2500)
  }

  /**
   * disable element
   * @param {element} ele
   * @param {boolean} isOk
   */
  static disableEle (ele, isOk) {
    if (isOk) {
      $(ele).attr('disabled', false)
    } else {
      $(ele).attr('disabled', isOk ? '' : 'disabled')
    }
  }

  static getFormJson = fm => {
    let arry = fm.serializeArray(),
      n = {}
    for (let i = 0; i < arry.length; i++) {
      if (arry[i].name.toLocaleLowerCase() == 'isshow') {
        arry[i].value = 'on' ? true : false
      }
      n[arry[i].name] = arry[i].value || ''
    }
    return n
  }

  static clnForm (formCls) {
    let fm = new Object()
    if (formCls && $('form.' + formCls).length > 0) {
      fm = $('form.' + formCls)
    } else {
      fm = document.getElementsByTagName('form')
    }
    let inp = $(fm).find('input,textarea')
    for (let index = 0; index < inp.length; index++) {
      const element = inp[index]
      if ($(element).attr('type') !== 'hidden') {
        $(element).val('')
      }
    }

    let selc = $(fm).find('select')
    for (let index = 0; index < selc.length; index++) {
      const element = selc[index]
      $(element).val(-1)
    }
    let ckb = $(fm).find('[type=checkbox]')
    for (let index = 0; index < ckb.length; index++) {
      const element = ckb[index]
      $(element).prop('checked', false)
    }
  }
}
