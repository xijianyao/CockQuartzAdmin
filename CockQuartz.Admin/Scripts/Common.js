//对象转json
(function ($) {
    $.fn.serializeJson = function () {
        var serializeObj = {};
        var array = this.serializeArray();
        $(array).each(function () {
            if (serializeObj[this.name]) {
                if ($.isArray(serializeObj[this.name])) {
                    serializeObj[this.name].push(this.value);
                } else {
                    serializeObj[this.name] = [serializeObj[this.name], this.value];
                }
            } else {
                serializeObj[this.name] = this.value;
            }
        });
        return serializeObj;
    };
})(jQuery);
var Requests = {
    cacheDatas: {},//内容包括id,param
    CachePost: function (url, param, callback) {
        var self = this;
        if (param in self.cacheDatas) {
            callback(self.cacheDatas[param]);
            return false;
        }
        $.ajax({
            type: "post",
            data: param,
            url: url,
            contentType: "application/json;charset=UTF-8",
            async: false,
            cache: false,
            success: function (data) {
                self.cacheDatas[param] = data;
                callback(data);
            }
        });
        return false;
    },
    Post: function (url, param, callback) {
        $.ajax({
            url: url,
            data: param,
            type: "POST",
            async: false,
            cache: false,
            contentType: "application/json;charset=UTF-8",
            success: function (data) {
                callback(data);
            }
        });
    },
    AsnyCachePost: function (url, param, callback) {
        var self = this;
        if (param in self.cacheDatas) {
            callback(self.cacheDatas[param]);
            return false;
        }
        loadingFunction();
        $.ajax({
            async: true,
            url: url,
            type: 'POST',
            cache: false,
            data: param,
            contentType: "application/json;charset=UTF-8",
            success: function (data) {
                self.cacheDatas[param] = data;
                callback(data);
                removeLoading();
            },
            error: function () {
                alert("网络或程序异常，请稍后再试");
                removeLoading();
            }
        });
    },
    AsnyPost: function (url, param, callback) {
        loadingFunction();
        $.ajax({
            async: true,
            url: url,
            type: 'POST',
            cache: false,
            data: param,
            contentType: "application/json;charset=UTF-8",
            success: function (data) {
                callback(data);
                removeLoading();
            },
            error: function () {
                alert("网络或程序异常，请稍后再试");
                removeLoading();
            }
        });
    },
    FormDataPost: function (url, formData, callback) {
        $.ajax(
            {
                async: true,
                url: url,
                type: 'POST',
                data: formData,
                // async: false,
                cache: false,
                /**
                *必须false才会自动加上正确的Content-Type
                */
                contentType: false,
                /**
                * 必须false才会避开jQuery对 formdata 的默认处理
                * XMLHttpRequest会对 formdata 进行正确的处理
                */
                processData: false,
                success: function (data) {
                    callback(data);
                },
                error: function (data) {
                    alert(data);
                    return false;
                }
            });
    },
    Get: function (url, callback) {
        $.ajax({
            async: false,
            type: "GET",
            url: url,
            cache: false,

            success: function (data) {
                callback(data);
            },
            error: function (data) {
                alert("网络或程序异常，请稍后再试");
                return false;
            }
        })
    }
};
var formatVerify = {
    phoneVerify: function (phone) {
        var reg = !(/^\d{7,15}$/.test(phone));
        if (phone.length === 0) {
            return "手机号码为空";
        } else if (phone.length < 7 || phone.length > 15) {
            return "请输入7到15位手机号码";
        } else if (reg) {
            return "手机号码格式错误";
        }
        return "";
    },
    idcardVerify: function (idcard) {
        var reg = !(/^\d{15,18}$/.test(idcard));
        if (idcard.length === 0) {
            return "身份号码为空";
        } else if (idcard.length < 15 || idcard.length > 18) {
            return "请输入15到18位身份号码";
        } else if (reg) {
            return "身份号码格式错误";
        }
        return "";
    },
    imageCodeVerify: function (imageCode) {
        var reg = !(/^\d{4}$/.test(imageCode));
        if (imageCode.length === 0) {
            return "请输入图形验证码";
        } else if (reg) {
            return "图形验证码格式错误";
        }
        return "";
    }
};
var Alert = {
    Result: function (param, type) {
        $.alert({
            wrapClassName: '.customer-class',
            type: type,
            title: param,
            text: '',
            enableOk: 1,
            okText: '确定',
            enableCancel: 1,
            cancelText: '取消',
            timer: 0,
            confirmLoading: 0,
            onOk: function () { },
            onCancel: function () { }
        });
    },
    SubConfirm: function (param, type, onok, oncancel,text) {
        $.alert({
            wrapClassName: '.customer-class',
            type: type,
            title: param,
            text: text,
            enableOk: 1,
            okText: '确定',
            enableCancel: 1,
            cancelText: '取消',
            timer: 0,
            confirmLoading: 0,
            onOk: onok,
            onCancel: oncancel
        });
    }
};
//分页
var Page = {
    Initialise: function (PagePanelId, PageCount, pageIndex, url, param, exhibitionId) {
        laypage({
            cont: PagePanelId,
            skip: true,
            pages: PageCount,
            first: 1,
            curr: pageIndex,
            last: PageCount,
            jump: function (obj, first) {
                if (!first) {
                    Page.PageJump(obj, param, url, "#" + exhibitionId);//分页方法
                }
            }
        });
    },
    PageJump: function (obj, param, url, exhibitionId) {
        param["pageIndex"] = obj.curr;
        var pageId = obj.cont;//放页面的pagePanel的id
        var curr = obj.curr;
        var pages = obj.pages;//页面总数，页面最后一页的页码
        Requests.Post(url, JSON.stringify(param), function (data) {
            $(exhibitionId).html(data);
            laypage({
                cont: pageId,
                skip: true,
                pages: pages,
                first: 1,
                curr: curr,
                last: pages,
                jump: function (obj, first) {
                    if (!first) {
                        Page.PageJump(obj, param, url, exhibitionId);//分页方法
                    }
                }
            });
        });
    }
};