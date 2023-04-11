class BiscMenu {
  static titleAttr = 'data-title'
  /**
   * @param {order numbr} order
   * @param {jquery dom} jqEle
   * @param {title} title
   */
  constructor(order, jqEle) {
    this.orderNo = order
    this.jqEle = jqEle
    this.title = $(this.jqEle).attr(BiscMenu.titleAttr)
  }


}

let
  MAIN_DIV = 'main_div',//dom id for the main view
  AppHost = 'http://localhost:5002'

/**
 * Nav of the Sidebar
 */
class SidebarModel extends BiscMenu {
  static OutDiv = $('#sidebar_module')
  /**
   * 
   * @param {*} order 
   * @param {*} jqEle 
   * @param {*} active 
   */
  constructor(order, jqEle, active = false) {
    super(order, jqEle)
    this.active = active
    this.routes = {}
    this._route = '' //route will be array with key value pairs.
    this.httpAction = new HttpAction()
  }

  get routeTitle() {
    return this._route
  }

  set routeTitle(navTxt) {
    if (navTxt) {
      this._route = navTxt
    } else {
      this._route = $(this.jqEle).attr('data-route')
    }
  }

  /**
   * get all urls of a sidebar just in tstppr console
   * @param {apiurl object } object
   */
  resetRoute(object) {
    for (const key in object) {
      if (Object.hasOwnProperty.call(object, key)) {
        const element = object[key]

        if (key == this.title) {
          this.routes = element
        } else if (this.title == '年份' || this.title == '省份') {
          this.routes = object['属性']
        }
      }
    }
    return null
  }
}

class MenuModuleModel extends BiscMenu {
  constructor(order, jqEle, outDiv = '#sidebar_module') {
    super(order, jqEle)

    this.title = $(jqEle)
      .find('button')
      .text()
      .trim()
    this.SidebarArray = []
    this.collectSidebars();
    this.outerDiv = outDiv
  }


  /**
   *
   * @param {module selector} ele
   * @param {order no.} module_index
   */
  collectSidebars() {

    let _slf = this
    $.each($(this.jqEle).find('li'), (i, e) => {
      _slf.SidebarArray.push(new SidebarModel(i, e));
    })
  }

  get moduleCtrl() {
    return $(this.jqEle).attr('data-ctrl')
  }

  get activeNav() {
    $.each(this.SidebarArray, (i, d) => {
      if (d.active == true) {
        return d
      }
    })
    return null
  }

  /**
   * get sidebar from all modules
   * @param {nav text} title
   * @returns
   */
  getLi(title) {
    if (this.SidebarArray.length > 0) {
      this.SidebarArray.find(x => {
        return x.title == title
      })
    } else {
      return null
    }
  }


}

class OperResultDialog {
  static ENV_Class = {
    default: 'bg-default',
    info: 'bg-info',
    success: 'bg-success',
    warning: 'bg-warning',
    danger: 'bg-danger'
  }

  constructor(isOk = true, msg = '', cls = OperResultDialog.ENV_Class.info) {
    this.isOk = isOk
    this.msg = msg
    this.cls = cls
  }

  _creatDialog() {
    let clsObj = {},
      bg = '',
      txt = 'text-white'
    switch (this.cls) {
      case OperResultDialog.ENV_Class.default:
        bg = 'bg-default'
        break
      case OperResultDialog.ENV_Class.info:
        bg = 'bg-info'
        break
      case OperResultDialog.ENV_Class.success:
        bg = 'bg-success '
        break
      case OperResultDialog.ENV_Class.warning:
        bg = 'bg-warning'
        txt = 'text-dark'
        break
      case OperResultDialog.ENV_Class.danger:
        bg = 'bg-danger'
        break
      default:
        break
    }
    clsObj = { class: 'tips fs-4 fw-bolder ' + bg + ' ' + txt }
    return DomGen.createDom('div', clsObj, this.msg)
  }
  static confirm(msg, cb = null) {
    dialog({
      title: '消息',
      content: msg,
      okValue: '嗯',
      ok: function () {
        if (cb) cb()
      },
      cancelValue: '取消',
      cancel: function () { }
    }).show()
  }

  popUp() {
    let _sf = this,
      d = dialog({
        title: '消息',
        content: _sf.msg,
        skin: 'min-dialog ' + _sf.cls
      })
    d.show()
    setTimeout(function () {
      d.close().remove()
    }, 2000)
  }

  showDialog(dialogId) {
    let dom =
      document.getElementById(dialogId) || document.querySelector(dialogId)
    if (!dom) {
      alert("DOM with '" + dialogId + "' not found!")
      return false
    }

    console.log('start showing modal')
  }

  showAlert(alertDivId) {
    let alertDiv = $('.' + alertDivId) || $('#' + alertDivId),
      clsOk = 'alert-success',
      clsError = 'alert-danger'

    if (alertDiv.length == 0) {
      alert("DOM with '" + alertDivId + "' not found!")
      return false
    }
    if (this.isOk) {
      if (!alertDiv.find('p').hasClass(clsOk)) {
        alertDiv.find('p').addClass(clsOk)
      }
    } else {
      if (!alertDiv.find('p').hasClass(clsError)) {
        alertDiv.find('p').addClass(clsError)
      }
    }

    alertDiv.find('p').html(this.msg)
    alertDiv.slideDown('fast')
    setTimeout(() => {
      alertDiv
        .slideUp('fast')
        .find('span')
        .empty()
    }, 3000)
  }
}

// form data modal with richtext container and jquery upload container

class FmModel {
  static richTxtContainer = 'container'
  static uploaderContainer = 'cvrUpload'

  /**
   *
   * @param {sidebar title} menu
   * @param {submit api} sbmitUrl
   * @param {edit id} id
   * @param {foreign key} fk
   * @param {class that trigger the event} slfObj
   */
  constructor(menu, sbmitUrl, id, fk) {
    this.menu = menu
    this.submitUrl = sbmitUrl
    this.id = id
    this.fk = fk
  }

  // fmCallback() {
  //   FmModel.ueInit()
  // }

  // static uploaderInit(cur_id) {
  //   //init jq_upload
  //   if (!FmModel.uploaderContainer) {
  //     return
  //   }

  //   //The default is image uploading.
  //   let pdfId = $('[name=id]').val(),
  //     uploader = new JqUploader(
  //       cur_id,
  //       FmModel.uploaderContainer,
  //       '封面图片',
  //       1,
  //       0,
  //       { pdf_id: pdfId, uploadType: 0 }
  //     )
  //   uploader.init({
  //     acceptFiles: 'image/*',
  //     showDelete: true,
  //     showPreview: true,
  //     dragDropStr: '',
  //     autoSubmit: true,
  //     deleteCallback: function (data, pd) {
  //       PdfPage.delFile(pdfId, 0, data)
  //       pd.statusbar.hide()
  //     },
  //     onSuccess: function (files, data, xhr, pd) {
  //       $('[name=image_path]').val(data)
  //     },
  //     onLoad: function (obj) {
  //       uploader.requestFiles(r => {
  //         if (r.path) obj.createProgress(r.name, r.path, r.size)
  //         else console.log('no cover image')
  //       })
  //     }
  //   })
  // }

  static ueInit() {
    console.log('richtext editor ')
    if (FmModel.richTxtContainer == null) {
      return
    } else {
      UE.delEditor(FmModel.richTxtContainer)
      let ued = UE.getEditor(FmModel.richTxtContainer, {
        toolbars: [
          [
            'fullscreen',
            'source',
            '|',
            'undo',
            'redo',
            '|',
            'bold',
            'italic',
            'underline',
            'fontborder',
            'strikethrough',
            'superscript',
            'subscript',
            'removeformat',
            'formatmatch',
            'autotypeset',
            'blockquote',
            'pasteplain',
            '|',
            'forecolor',
            'backcolor',
            'insertorderedlist',
            'insertunorderedlist',
            'selectall',
            'cleardoc',
            '|',
            'rowspacingtop',
            'rowspacingbottom',
            'lineheight',
            '|',
            'customstyle',
            'paragraph',
            'fontfamily',
            'fontsize',
            '|',
            'directionalityltr',
            'directionalityrtl',
            'indent',
            '|',
            'justifyleft',
            'justifycenter',
            'justifyright',
            'justifyjustify',
            '|',
            'touppercase',
            'tolowercase',
            '|',
            'link',
            'unlink',
            'anchor',
            '|'
          ]
        ],
        initialFrameHeight: 300,
        enableAutoSave: false,
        initialFrameWidth: $('#' + FmModel.richTxtContainer)
          .parents('div')
          .width()
      })
      ued.ready(function () {
        let divCnt = $('#cnt').text()
        ued.setContent(divCnt, false)
      })
      return ued
    }
  }
}

class PagedModel {
  constructor(menu, ord, isAsc = true, pg = 1) {
    this.pg = pg
    this.orderby = ord
    this.isAsc = isAsc
    this.menu = menu
  }
}

class PdfPagedModel extends PagedModel {
  constructor(menu, ord, isAsc, pg, bid) {
    super(menu, ord, isAsc, pg)
    this.bindid = bid
    this.ReqUrl = AppHost + '/api/pdf/page'
    this.pdfDom = '#pdf_div'
    this.httpClnt = new HttpAction(this.pdfDom)
  }

  static GenPdfTable(tableData) {
    if (tableData) {
      let pdfTable = $('#pdf_div>table'),
        tb = DomGen.createDom('tbody')
      $(tb).html(tableData.tableRowDom)
      $(pdfTable)
        .find('tbody')
        .eq(0)
        .empty()
        .append(tb.innerHTML)

      if ($('div.black2').length == 0) {
        let d = DomGen.createDom('div', { class: 'black2 pager' })
        $('table').after(d)
      }
      $('div.black2').html(tableData.pagerDom)
    }
  }

  genTables(data) {
    if (data && data.table) {
      PdfPagedModel.GenPdfTable(data.table)
    }
  }

  //load new page by current bindid
  load_data() {
    this.httpClnt.requests(this.ReqUrl, this, d => this.genTables(d), 'post')
  }
}

let GetPageList = function (pg, ttl, tp) {
  //tp:itemcategs(tp<4)|pdf list(tp=4)
  let routes = BaseReqModel.Routes,
    lastData = routes[routes.length - 1]

  if (lastData) {
    if (!lastData.data) {
      console.log('[**Error**] sidebar title can not be found')
      return
    }
    let data = JSON.parse(lastData.data)
    data.pg = pg
    let mc = new MenuController(MAIN_DIV)
    $('table thead')
      .find('input[type=checkbox]')
      .prop('checked', false)
    if (tp == 4 && data.menu.indexOf('电子书') !== -1) {
      let bid = $('[name=bindid]').val(),
        pdfMdl = new PdfPagedModel(data.menu, 'id', true, pg, bid)

      pdfMdl.load_data()
    } else {
      mc.requestView(lastData.url, data, data)
    }
  }
}

class BaseReqModel {
  /**
   * request history of routes
   */
  static Routes = []
  constructor(outid) {
    this.outId = outid
    this.popUp = new OperResultDialog()
  }

  /**
   * log request url if it's changing the route.
   * @param {main page url} url
   * @param {parms of the request} data
   */
  _route_log = (url, data, cb, mtd = 'get') => {
    if (String(url).indexOf('/api/') !== -1) {
      return
    }
    let cur = new Route(url, data, mtd, cb)

    let prv =
      BaseReqModel.Routes.length > 0
        ? BaseReqModel.Routes[BaseReqModel.Routes.length - 1]
        : ''

    if (prv == '' || !prv.IsEqual(cur)) {
      BaseReqModel.Routes.push(cur)
    }
  }

  /**
   * request api
   * @param {*} route
   * @param {*} data
   * @param {*} cb
   * @param {*} method
   * @returns
   */
  request_data = (route, data, cb, method = 'get') => {
    method = method.toLocaleLowerCase()
    let requestHeader = {
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json'
      },
      method
    }
    if (method === 'post' || method === 'put') {
      requestHeader.body = JSON.stringify(data)
    }

    return fetch(route, requestHeader)
      .then(
        rs => {
          this._route_log(route, data, method)
          return rs.json()
        },
        rj => {
          console.log(rj)
        }
      )
      .then(response => {
        if (cb) {
          cb(response)
        }
      })
      .catch(error => {
        console.log(error)
      })
  }

  /**
   *
   * @param {view url} route_url
   * @param {requset data}  data
   * @param {callback func i.e menuClr.viewCb } cb
   * @param { callback params } dataCb
   * @param {*} method
   */
  request_view = (route_url, data, cb, dataCb, method = 'get') => {
    if (!route_url) {
      return false;
    }

    let _slf = this
    $.ajax({
      url: route_url,
      type: method,
      data: data,
      success: r => {
        this._route_log(route_url, data, method)
        let d = document.getElementById(_slf.outId)
        if (!d) {
          console.log(`[*error*]: no element with id ${_slf.outId} found!`)
        }
        if (d && r) {
          d.innerHTML = r
        }
        //cb should be menuClr.viewCb.

        if (cb) cb(dataCb)
      }
    })
  }
}


