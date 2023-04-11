/**
 * real value for the Routes array.
 */
class Route {
  constructor(url, data, mtd = 'get') {
    this.url = url
    /**
     * page model data
     */
    this.data = JSON.stringify(data)
    this.method = mtd
    if (this.data) this.join_value = this.url + '/' + this.data
    else {
      this.join_value = this.url
    }
  }

  IsEqual(rt) {
    if (rt.join_value) {
      return rt.join_value === this.join_value
    }
  }

  static genRoute(eleLi) {
    let pmd = new PagedModel(eleLi.title, 'orderNo', true, 1)
    return new Route(eleLi.route, pmd)
  }
}


/**
 * view request model
 */
class ReqViewModel extends BaseReqModel {
  constructor(outId, ajaxUrl, reqModel, cb, restParam = null) {
    super(outId)
    this.ajaxUrl = ajaxUrl
    this.reqModel = reqModel
    //mostly it is an object or a kind of model
    this.restParam = restParam
  }
}

class HttpAction extends BaseReqModel {
  constructor(outId) {
    super()
    this.outId = outId
    this.dashboardUrl = '/dashboard/'
    this.toolFunc = new ToolFunc(outId)
    this.model_id = 'modal_tip'
  }

  get tstppr_urls() {
    return {
      属性: {
        defaultViewSf: '/testpaper/props/sf',
        defaultViewNf: '/testpaper/props/nf',
        editView: '/testpaper/propertyEdit',
        api: '/api/testpaper/props'
      },
      创建栏目: {
        defaultView: '/testpaper/columncreate',
        api: '/api/testpaper',
        apiChildren: '/api/testpaper/children/'
      },
      栏目列表: {
        defaultView: '/testpaper/columnlist',
        editView: '/testpaper/columnedit',
        api: '/api/testpaper'
      },
      上传试卷: {
        defaultView: '/testpaper/uploadedit',
        editView: '/testpaper/UploadPage',
        api: '/api/testpaper/tpUploadEdit',
        uploadApi: '/api/testpaper/tpUpload'
      },
      试卷列表: {
        listView: '/testpaper/uploadFileList',
        api: '/api/testpaper/'
      }
    }
  }

  get pdf_urls() {
    let _host = AppHost + this.dashboardUrl
    return {
      get_categsV: _host + '_RgtTblView',
      get_booksV: _host + '_RgtBooksView',
      get_bdingV: _host + '_RgtBindView',
      get_tableAddEditV: _host + '_CategoryFormView',
      api_itemCateg: AppHost + '/api/itemcategory',
      api_itemCateg_preDel: AppHost + '/api/itemcategory/predel',
      api_pdf: AppHost + '/api/pdf',
      get_pdfNewEditV: AppHost + '/pdf'
    }
  }

  static async formSubmit(sbMitUrl, fmCls, mth = 'post') {
    const fm = $('.' + fmCls),
      fmdata = ToolFunc.getFormJson(fm),
      headers = new Headers(),
      options = {
        method: mth,
        headers,
        body: JSON.stringify(fmdata)
      }

    headers.append('Content-Type', 'application/json')
    headers.append('Accept', 'application/json')
    // console.log(fmdata)
    return await fetch(sbMitUrl, options)
      .then(res => res.json())
      .then(data => ({ data }))
      .catch(error => ({ error }))
  }

  /**
   * request pag or data.
   * @param {request partial route} route
   * @param {request model} data
   * @param {cb} cb
   * @param {type of the mtd} method
   * @returns
   */
  requests = (route, data = {}, cb = null, method = 'GET', dataCb = null) => {
    let _slf = this
    if (!route) {
      return
    }

    let dataReq = String(route).includes('/api/')
    if (!dataReq) {
      _slf.request_view(route, data, cb, dataCb)
    } else {
      _slf.request_data(route, data, cb, method)
    }
  }
}
