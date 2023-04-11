class MyForm {
  constructor (eleId) {
    this.formId = eleId
  }

  /**
   * submit form with uploader initialization.
   * @param {submit url} _url
   * @param {ele for upload id} uploadEleId
   */
  static formEvtUpload (_url, uploadEleId) {
    if (!_url || !uploadEleId) {
      console.log('url or uploader id is undefined')
      return
    }

    //init uploader
    let fmdata = function () {
        return {
          nf: parseInt($('[name=nf]').val()),
          sf: parseInt($('[name=sf]').val()),
          isAdd: $('#isAdd').val()
        }
      },
      upl = new JqUploader('', uploadEleId, '上传试题', 3)
    upl.init({
      url: _url,
      autoSubmit: false,
      showDelete: true,
      deleteStr: '删除',
      extraHTML: function () {},
      onSubmit: function (files, xhr) {
        upl.formData = fmdata()
        console.log(upl)
      }
    })

    $('#sbm').on('submit', e => {
      e.preventDefault()

      var _isAdd = $('.IsAdd').val()
      if (!_isAdd) {
        console.log('[error]:is add or edit is not sure?')
        return
      }

      upl.uploader.startUpload()
    })
  }
  /**
   * used for view callback if there are forms in page.
   * @param {submit url} _url
   */
  static formEvt = function (_url) {
    let forms = document.getElementsByTagName('form'),
      _isAdd = 0
    if (forms.length == 0) {
      console.log('[error]:no forms found')
      return
    }
    $(forms).each(function () {
      let f = this
      $(f).on('submit', function (e) {
        e.preventDefault()
        _isAdd = $(f)
          .find('.IsAdd')
          .val()
        if (!_isAdd) {
          console.log('[error]:is add or edit is not sure?')
          return
        }
        if (_url) {
          
          HttpAction.formSubmit(_url, $(f).attr('id')).then(d => {
            let dig = new OperResultDialog()
            if (d.data && d.data !== 0) {
              let msg = '增加了1条记录'
              if (_isAdd !== '1') {
                msg = '更新了1条记录'
              } else {
                let idInput = $('input[name=id]')
                if (idInput.length > 0) {
                  $('input[name=id]').val(d.data)                 
                } else {
                  console.log('[*]:id dom not found')
                }
              }
              dig.isOk = true
              dig.msg = msg
              dig.cls = OperResultDialog.ENV_Class.success
            } else {
              dig.isOk = false
              dig.msg = '保存失败'
              dig.cls = OperResultDialog.ENV_Class.danger
            }
            dig.popUp()
          })
        } else {
          console.log('[*error*]:url is none when form submits')
        }
      })
    })
  }
  //收集表单数据为一个数组
  static request (name) {
    var search = location.search.slice(1)
    var arr = search.split('&')
    for (var i = 0; i < arr.length; i++) {
      var ar = arr[i].split('=')
      if (ar[0] == name) {
        if (ar[1].toLocaleLowerCase() === 'undefined') {
          return ''
        } else {
          return ar[1]
        }
      }
    }
    return ''
  }
}

$.fn.formSerialize = function (formData) {
  var element = $(this)
  if (!!formData) {
    for (var key in formData) {
      var $id = element.find('#' + key)
      var value = $.trim(formData[key]).replace(/&nbsp;/g, '')
      var type = $id.attr('type')
      if ($id.hasClass('select2-hidden-accessible')) {
        //if ($id.hasClass("form-select")) {
        type = 'select'
      }
      switch (type) {
        case 'checkbox':
          if (value == 'true') {
            $id.attr('checked', 'checked')
          } else {
            $id.removeAttr('checked')
          }
          break
        case 'select':
          $id.val(value).trigger('change')
          break
        default:
          $id.val(value)
          break
      }
    }
    return false
  }
  let postdata = {}
  element.find('input,select,textarea').each(function (r) {
    let $this = $(this)
    let id = $this.attr('id')
    let type = $this.attr('type')
    switch (type) {
      case 'checkbox':
        postdata[id] = $this.is(':checked')
        break
      default:
        let value = $this.val() == '' ? '&nbsp;' : $this.val()
        if (!MyForm.request('keyValue')) {
          value = value.replace(/&nbsp;/g, '')
        }
        postdata[id] = value
        break
    }
  })
  if ($('[name=__RequestVerificationToken]').length > 0) {
    postdata['__RequestVerificationToken'] = $(
      '[name=__RequestVerificationToken]'
    ).val()
  }
  return postdata
}
