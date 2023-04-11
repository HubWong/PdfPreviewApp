/**
 * docs :https://github.com/blueimp/jQuery-File-Upload/wiki/API
 * with jq uploader
 */
class JqUploader {
  static api_uploadUrl = '/api/upload/fileupload'

  constructor(dataPrimaryKey, domId, uploadStr, maxFileCount, type, formData) {
    this.id = dataPrimaryKey
    this.dom_id = domId
    this.uploadStr = uploadStr
    this.maxFileCount = maxFileCount
    this.uploadType = type
    this.formData = formData
    this.apiClnt = new HttpAction() //start requesting.
    this.uploader = {};
    this.items = []
  }

  /**
   * get preview url item
   */
  _getPrvItem(item) {
    let link = '', title = '';
    if (item && item.name) {
      const reg = /(http:\/\/|https:\/\/)((\w|=|\?|\.|\/|&|-)+)/g;
      link = item.name.match(reg)[0],
        title = String(item.name)
          .replace(link, '')
          .replace('[', '')
          .replace(']', '');
    }
    return [link, title]
  }
  getExtraForPrv = ({ link, title }) => {

    let rowDiv = DomGen.createDom('div', { 'class': 'row mb-2', 'style': 'flex-direction:column' }),
      lftLbl = DomGen.createDom('label', { 'class': 'col-sm-3 col-form-label' }, '链接'),
      rgtDiv = DomGen.createDom('div', { 'class': 'col-sm-9' }),
      inp = DomGen.createDom('input', { 'name': 'destUrl', 'class': "form-control", 'type': 'url', 'required': 'required', 'placeholder': "请输入http(s)地址", 'value': link | '' });

    rgtDiv.append(inp);
    rowDiv.append(lftLbl);
    rowDiv.append(rgtDiv);

    let lftLbl2 = DomGen.createDom('label', { 'class': 'col-sm-3 col-form-label' }, '名称'),
      inp2 = DomGen.createDom('input', { 'name': 'title', 'class': "form-control", 'type': 'text', 'required': 'required', 'placeholder': "请输入名称", 'value': title | '' }),
      rgtDiv2 = DomGen.createDom('div', { 'class': 'col-sm-9' })
      ;


    rowDiv.append(lftLbl2);
    rgtDiv2.append(inp2)
    rowDiv.append(rgtDiv2);

    return rowDiv.outerHTML;
  }
  requestFiles(cb, param) {
    if (!param) {
      return
    }
    this.apiClnt.requests(AppHost + this.fileTypeUrl, param, cb, 'get')
  }

  /**
   * get url by uploading type
   */
  get fileTypeUrl() {
    let _fileType = 0
    switch (this.dom_id) {
      case 'cvrUpload':
        _fileType = '0'
        break
      case 'fileUpload':
        _fileType = '1'
        break
      case 'urlCvrUpload':
        _fileType = '2'
        break
      case 'tpUploader':
        _fileType = '3'
    }
    return '/api/pdf/files/' + _fileType + '/' + this.id
  }

  get allowedTypes() {
    switch (this.dom_id) {
      case 'cvrUpload':
      case 'urlCvrUpload':
        return 'jpeg,jpg,gif,png'
      case 'fileUpload':
        return 'doc,docx,pptx,ppt,zip,rar'
      case 'tpUploader':
        return 'doc,docx'
    }
  }

  start() {
    if (this.uploader.startUpload) {
      this.uploader.startUpload()
    }
    else {
      console.log('start upload is null')
    }
  }

  init(op) {
    let sf = this,
      count = 0,
      thisOps = {
        url: JqUploader.api_uploadUrl,
        allowedTypes: sf.allowedTypes,
        //autoSubmit: sf.dom_id == 'fileUpload' ? false : true,
        showPreview: true,
        showDelete: true,
        uploadStr: sf.uploadStr,
        cancelStr: '取消',
        dragDropStr: '',
        previewHeight: '70px',
        previewWidth: '70px',
        sizeErrorStr: '文件太大',
        extErrorStr: '扩展名错误,只允许:',
        maxFileCount: sf.maxFileCount,
        maxFileCountErrorStr: '不能超过上传数量:',
        sequential: true,
        sequentialCount: 1,
        deleteStr: '删除',
        formData: sf.formData,
        onSuccess: function (files, data, xhr, pd) {
          //files: list of files
          //data: response from server
          //xhr : jquer xhr object          
          $(pd['filename']).attr('data-id',data.id)
          sf.items.push(data)
          $(pd.extraHTML).addClass('hide')
        },
        customProgressBar: function (obj, s) {
          let domId = $(obj[0]).attr('id'),
            rightDiv = DomGen.createDom('div', { class: 'right' }),
            btmDiv = DomGen.createDom('div', { style: 'clear:both' });
          this.statusbar = $("<div class='ajax-file-upload-statusbar'></div>")
          const lst = sf.items.pop()
          if (domId === 'urlCvrUpload') {
            if (obj.existingFileNames.length == 0) { //it is preload the prvious           
              s.extraHTML = () => {
                let itm = sf._getPrvItem(lst)
                return itm[1]
              }
            } else {
              s.extraHTML = () => sf.getExtraForPrv('')
            }
          }
          if (s.allowedTypes.indexOf('jpg') !== -1) {
            this.preview = $(`<img class='ajax-file-upload-preview left' />`)
              .width(s.previewWidth)
              .height(s.previewHeight)
              .appendTo(this.statusbar)
              .hide()
          } else {
            this.preview = $('<div><div>')
              .appendTo(this.statusbar)
              .hide()
            rightDiv = DomGen.createDom('div')
          }

          this.filename = $(
            `<div class='ajax-file-upload-filename' data-id='${lst?.id}'></div>`
          ).appendTo(rightDiv)
          this.progressDiv = $("<div class='ajax-file-upload-progress'>")
            .appendTo(rightDiv)
            .hide()
          this.progressbar = $(
            "<div class='ajax-file-upload-bar'></div>"
          ).appendTo(this.progressDiv)
          this.abort = $('<div>' + s.abortStr + '</div>')
            .appendTo(rightDiv)
            .hide()
          this.cancel = $('<div>' + s.cancelStr + '</div>')
            .appendTo(rightDiv)
            .hide()
          this.done = $('<div>' + s.doneStr + '</div>')
            .appendTo(rightDiv)
            .hide()
          this.download = $('<div>' + s.downloadStr + '</div>')
            .appendTo(rightDiv)
            .hide()
          this.del = $('<div>' + s.deleteStr + '</div>')
            .appendTo(rightDiv)
            .hide()
          $(rightDiv).appendTo(this.statusbar)
          $(btmDiv).appendTo(this.statusbar)
          this.abort.addClass('ajax-file-upload-red')
          this.done.addClass('ajax-file-upload-green')
          this.download.addClass('ajax-file-upload-green')
          this.cancel.addClass('ajax-file-upload-red')
          this.del.addClass('ajax-file-upload-red btn-sm')
          if (count++ % 2 === 0) this.statusbar.addClass('odd')
          else {
            this.statusbar.addClass('even')
          }


          return this
        }
      }

    Object.assign(thisOps, op)

    if ($('#' + this.dom_id).length > 0) {
      this.uploader = $('#' + this.dom_id).uploadFile(thisOps)
    } else {
      console.log('[**]:not element found,jq uploader cannot be created')
    }
  }
}
