/**
 * view doc at://http://hayageek.com/docs/jquery-upload-file.php#customui
 * pdf page list
 * with top itemcategories and pdf list by bindid.
 * pdf list(table) could be paged or sorted by column selected. 
 */
class PdfPage {
    static cls_catch = 'catch';
    static title = '电子书列表';
    static dialogUploadOpend = false;
    static uploadCvrId = 'cvrUpload';
    static fileTypes = "doc,docx,pptx,ppt,zip,rar"
    static api_ReqBds = AppHost + '/api/loadBinding';
    static api_loadRestBds = AppHost + '/api/pdf/rest';
    static api_urlUpload = AppHost + '/api/pdf/upload';
    static api_pdf_files = AppHost + '/api/pdf/files/' //attaches and previews 
    static httpClnt = new HttpAction();

    constructor(sec_div) {
        this.sec_div = sec_div;
        this.tableFormater = new DomGen(null, 'pdf_div');
        this.bid = -1;
        this.bdModel = new BindingModel(PdfPage.title);
        this.pdfModel = new PdfPagedModel(PdfPage.title, 'makeDay', true, 1, this.bid)
        this.clkIndex = 0;
        this.mCrl = new MenuController()
    }

    static cvrMgr(el) {

        let cur_id = $(el).parents('tr').find(".id").text(),
            d = dialog({
                title: '封面',
                content: '<div id="cvrUpload">fileupload</div>',
                onshow: function () {
                    PdfPage.dialogUploadOpend = true;
                    let uploader = new JqUploader(cur_id, 'cvrUpload', '封面上传', 1, 0, { pdf_id: cur_id, uploadType: 0 });
                    uploader.init({
                        acceptFiles: "image/*",
                        showDelete: true,
                        deleteStr: "删除",
                        showPreview: true,
                        dragDropStr: '',
                        autoSubmit: true,
                        deleteCallback: function (data, pd) {
                            PdfPage.delFile(0, cur_id)
                            pd.statusbar.hide();
                        },

                        onSuccess: function (files, data, xhr, pd) {
                            if (data)
                                $('[name=image_path]').val(data);
                        },
                        onLoad: function (obj) {
                            $.ajax({
                                cache: false,
                                url: PdfPage.api_pdf_files + `${0}/${cur_id}`,
                                dataType: "json",
                                success: function (data) {
                                    if (data === 0) {
                                        return;
                                    }
                                    const jsData = data
                                    uploader.items.push(data)
                                    obj.createProgress(jsData.name, jsData.path, jsData.size)
                                }
                            });
                        }
                    });
                },
                ok: function () {

                },
                cancelValue: '取消'
            })

        d.addEventListener('close', function () {
            PdfPage.dialogUploadOpend = false;
            d.remove();
        });
        d.showModal();
    }

    /**
     * clicked event
     * @param {button in the tr} el 
     */
    static pdfFilesPage(el) {
        let _id = $(el).parents('tr').find(".id").text(),
            v_url = AppHost + '/pdf/attach/' + _id,
            mCrl = new MenuController(),
            uploadInit = () => {
                PdfPage.attach(_id);
                PdfPage.preview(_id);
            };
        MenuController.cur_id = _id;
        mCrl.httpAction.request_view(v_url, null, () => uploadInit());

    }

    //if path is the same, set ele with id. 
    static setNodeAttrId(data, path) {

        if (data) {
            let d = data.find(p => {
                return p.path === path;
            });
            if (d) {
                return d.id;
            }
        }
        return ''
    }
    /**
    * adding preview url 
    * this func need pdfid
    */
    static attach(id) {  //attaches uploading
        let cur_id = MenuController.cur_id,
            uploader = new JqUploader(cur_id, 'fileUpload', '附件上传', 5, 1, { pdf_id: cur_id, uploadType: 1 });
        uploader.init({
            autoSubmit: false,
            showDelete: true,
            deleteStr: "删除",
            deleteCallback: function (data, pd) {                
                let id = $(pd['filename']).attr('data-id');

                if (id) {
                    PdfPage.delFile(1, id);
                    pd.statusbar.hide();
                } else {
                    throw Error('id not found when delete attach')
                }

            },
            onLoad: function (obj) { //when the jq uploader initializing.
                $.ajax({
                    cache: false,
                    url: PdfPage.api_pdf_files + `${1}/${id}`,
                    dataType: "json",
                    success: function (data) {

                        for (var i = 0; i < data.length; i++) {
                            uploader.items.push(data[i])
                            obj.createProgress(data[i]["name"], data[i]["path"], data[i]["size"], data[i]['id']);
                        }
                    }
                });
            },
        });
        $('#btn_attach').on('click', function () {
            uploader.uploader.startUpload();
        })
    }

    static preview(id) {
        let
            //instance of jquploader,due to upload event needs clicking.
            uploader = new JqUploader(MenuController.cur_id, 'urlCvrUpload', '新增预览', 10, 2, { pdf_id: MenuController.cur_id, uploadType: 2 });

        uploader.init({
            allowedTypes: "jpg,jpeg,gif,png",
            autoSubmit: false,
            deleteStr: "删除",
            extraHTML: function (f) {
                return uploader.getExtraForPrv();
            },         
            deleteCallback: function (data, pd) {                        
                
                let id = $(pd['filename']).attr('data-id');
                if (id) {
                    PdfPage.delFile(2, id);
                } else {
                    throw Error('id not found when delete preview item.')
                }

            },
            onLoad: function (obj) {
                $.ajax({
                    cache: false,
                    url: PdfPage.api_pdf_files + `${2}/${id}`,
                    dataType: "json",
                    success: function (data) {
                        for (var i = 0; i < data.length; i++) {
                            uploader.items.push(data[i])
                            obj.createProgress(data[i]["name"], data[i]["path"], data[i]["size"]);
                        }
                    }
                });
            }
        });

        $('#btn_preview').on('click', function () {
            let fm = $('div.ajax-file-upload-statusbar'),
                isOk = true;
            for (let i = 0; i < fm.length; i++) {
                const el = fm[i],
                    cnclBtnDiv = $(el)
                        .find('div.ajax-file-upload-cancel').attr('style');


                if (!$(el).find('extrahtml').hasClass('hide') && cnclBtnDiv == '') { //extrhtml hiden, cancel shows.
                    let
                        inp = $(el).find('input[name=destUrl]'),
                        inpV = inp.val(),
                        isUrl = ToolFunc.isUrl(inpV),
                        title = $(el).find('input[name=title]').val();

                    if (String(inpV).trim() == '') {
                        isOk = false;
                        let popD = new OperResultDialog(false, '请填写正确链接地址', OperResultDialog.ENV_Class.danger);
                        popD.popUp();
                        return;

                    } else if (String(title).trim().length === 0) {
                        isOk = false;
                        console.log('is 0')
                        let popD = new OperResultDialog(false, '请填写名称', OperResultDialog.ENV_Class.danger);
                        popD.popUp();
                    }

                    if (!isUrl) {
                        isOk = false;
                        let popD = new OperResultDialog(false, '请填写正确链接地址', OperResultDialog.ENV_Class.danger);
                        popD.popUp();
                        return;
                    }
                }


            }
            if (isOk) {
                //console.log(isOk)
                uploader.uploader.startUpload();
            }

        })
    }

    /**
     * delete preview, attach file or the whole record
     * @param { delete file type} type 
     * @param {record id} id 
     */
    static delFile(type, id) {
        let delFileApi;
        if (type === 0) {
            delFileApi = '/api/pdf/cvr/' + id;
        } else {
            delFileApi = '/api/pdf/file/' + type + '/' + id;
        }

        if (id && id !== 'undefined') {
            new HttpAction().requests(delFileApi, null, r => {
                if (r > 0) {
                    new OperResultDialog(true, '删除了' + r + '个文件').popUp();
                }
            }, 'delete')
        } else {
            new OperResultDialog(false, '删除时参数却少', 'bg-danger').popUp();
        }

    }

    static delCvr(pdfid) {
        let delCvrUrl = AppHost;
        if (pdfid) {
            delCvrUrl += '/api/pdf/cvr/' + pdfid;
        } else {
            delCvrUrl += '/api/pdf/cvr/' + $('[name=id]').val();
        }
        new HttpAction().requests(delCvrUrl, null, r => {
            if (r > 0) {
                new OperResultDialog(true, '删除了' + r + '个文件').popUp();
            }
            return r;
        }, 'delete')



    }

    getIndexNo(ele_a) {
        if (ele_a) {
            this.clkIndex = $(ele_a)
                .parents('div')
                .parent('section').index();
        } else {
            console.log('[**]:ele not defined');
        }
    }

    setDownTxt() {
        $("section").eq(this.clkIndex).nextAll().find('ul')
            .html(BindingModel.emptyDataLi);
    }

    init() {

        $('section').each(function () {
            let liDom = $(this).find("div.bd>ul").find('li');
            if (liDom.length !== 0) {
                $(this).find('li').eq(0).addClass(PdfPage.cls_catch)
            } else {
                $(this).find("div.bd>ul").html(PdfPage.emptyDataLi())
            }

        })
        this._evts();
    }

    _evts() {
        let _slf = this;
        $('section li>a').unbind('click')  //top buttons clicked for changing table
            .on('click', function () {
                $("table thead").find("input[type=checkbox]").prop("checked", false);
                _slf.getIndexNo(this);

                _slf.setDownTxt();
                $(this).parent()
                    .addClass(PdfPage.cls_catch)
                    .siblings().removeClass(PdfPage.cls_catch);
                let d = $(this).attr('rel');
                _slf.regenModel(d);
                _slf._load();
            });
    }



    _load() {
        this.mCrl.httpAction.request_data(PdfPage.api_loadRestBds, this.bdModel,
            r => {
                if (r) {
                    //rebuild table.
                    for (const key in r) {

                        if (Object.hasOwnProperty.call(r, key)) {
                            const el = r[key], k = key.toLocaleLowerCase();

                            if (k == 'binddata') {
                                this._restLevels(el)
                            } else if (k == 'table') {
                                PdfPagedModel.GenPdfTable(el);
                            }
                            else {
                                $('[name=bindid]').val(el);
                                this.bid = el;
                            }
                        }
                    }
                }
            }, 'post');

    }

    /**
     * load rest bds.
     * @param {xx_no} e 
     */
    regenModel(e) {
        if (e) {
            this.bdModel.genModel(e);
            if (this.bdModel.clk != e) {
                this.bdModel.clk = e;
            }
        }
    }

    _fill_bd_data(k, list) {
        let initDom = '';
        switch (k) {
            case 'mo_kuai':
                $('div.mk ul').empty();
                for (let index = 0; index < list.length; index++) {
                    const data = list[index], li = DomGen.createDom('li');

                    initDom = DomGen.createDom('a',
                        { 'rel': 'mk_' + data.id, 'href': 'javascript:;' },
                        data.title);
                    li.append(initDom);
                    $('div.mk ul').append(li);
                }

                break;
            case 'xue_ke':
                $('div.xk ul').empty();
                for (let index = 0; index < list.length; index++) {
                    const data = list[index], li = DomGen.createDom('li');

                    initDom = DomGen.createDom('a',
                        { 'rel': 'xk_' + data.id, 'href': 'javascript:;' },
                        data.title);
                    li.append(initDom);
                    $('div.xk ul').append(li);
                }
                break;
            case 'ban_ben':
                $('div.bb ul').empty();
                for (let index = 0; index < list.length; index++) {
                    const data = list[index], li = DomGen.createDom('li');

                    initDom = DomGen.createDom('a',
                        { 'rel': 'bb_' + data.id, 'href': 'javascript:;' },
                        data.title);
                    li.append(initDom);
                    $('div.bb ul').append(li);
                }
                break;
            default:
                break;
        }
    }

    _restLevels(data) {
        let _sf = this;
        if (data) {
            if (data['statusCode'] == -1) {
                console.log('[**]no bind data')
                return false;
            } else {
                for (let k in data) {
                    if (data[k].length > 0) {
                        _sf._fill_bd_data(k, data[k]);
                    }
                }

                //make the new dom selected by add class
                $('section:gt(' + this.clkIndex + ')').each(function () {
                    $(this).find("li").eq(0).addClass(PdfPage.cls_catch);
                })
                this._evts();
            }
        }
    }



}