

var ChartWidget = function (options) {
    this.chart = null;
    this.CHARTTYPES = {
        simple: 0,
        overlaid: 1,
        right: 2
    };
    this.displayValue = null;

    this.options = {
        currency: null,
        summed: false,
        formatValue: true,
        chartType: this.CHARTTYPES.overlaid,
        updateInterval: 15000,

        chartOptions: {
            credits: {
                enabled: false
            },
            chart: {
                renderTo: 'theChart',
                zoomType: 'x',
                spacingRight: 20,
                //backgroundColor:'#FFFFFF',
                backgroundColor: {
                    linearGradient: [0, 0, 0, 200],
                    stops: [
                        [0, 'rgba(255,255,255,0)'],
                        [1, 'rgba(255,255,255,0)']
                    ]
                }
            },
            title: {
                text: null
            },
            subtitle: {
                text: null
            },
            xAxis: {
                type: 'datetime',
                title: {
                    text: null
                },
                tickPosition: 'inside',
                tickColor: '#B0C0D0'
            },
            yAxis: {
                title: {
                    text: '  '
                },
                min: 0, startOnTick: false,
                showFirstLabel: false
            },
            tooltip: {
                enabled: true,
                shared: false,
                formatter: function () {

                    var dp = 1;
                    if (self.options.currency) {
                        dp = 2;
                    }

                    var tt = '';
                    if (this.series.name) {
                        tt += '<b>' + this.series.name + '</b><br/>';
                    }
                    tt += Highcharts.dateFormat('%Y-%m-%d %H:%M', this.x) + '<br/>' + self.insertCurrency(self.options.currency) + self.numberFormatter(this.y, dp);
                    return tt;
                }
            },

            legend: {
                enabled: false
            },

            plotOptions: {
                area: {
                    fillColor: {
                        linearGradient: [0, 0, 0, 200],
                        stops: [
                            [0, 'rgba(69, 114, 167, 0.8)'],
                            [1, 'rgba(230,230,230,0.2)']
                        ]
                    },
                    lineWidth: 1,
                    marker: {
                        enabled: false,
                        states: {
                            hover: {
                                enabled: true,
                                radius: 5
                            }
                        }
                    },
                    shadow: false,
                    states: {
                        hover: {
                            lineWidth: 1
                        }
                    }
                }
            },

            series: [
                {
                    type: 'area',
                    name: '',
                    data: []
                }
            ]
        }
    };


    $.extend(this.options, options);




    var self = this;

    self.displayValue = $('#' + options.id_scope + '_theValue');

    self.options.chartOptions.chart.events = { load: function () {
        self.onChartLoaded(self, this);
    }
    };

    if (self.options.report) {
        self.options.chartOptions.plotOptions.area.cursor = 'pointer';
        self.options.chartOptions.plotOptions.area.point = { events: { click: function () {
            if (self.options.report) {
                top.location.href = self.options.report + '?center=' + this.x;
            }
        }
        }
        };
    }


    Highcharts.setOptions({
        global: {
            useUTC: false
        }
    });
    self.chart = new Highcharts.Chart(self.options.chartOptions);

};


ChartWidget.prototype = function () {
    var self, lastUpdated;
    var clean = function (number) {
        if (!number) {
            return '';
        }
        return (number + '').replace('.0', '');
    },
        calculatePercentage = function (thisLevel, previousLevel, showDifference) {
            if (previousLevel != 0) {
                var perc = 100 * thisLevel / previousLevel;
                if (showDifference) {
                    perc = -1 * (100 - perc);
                }
                return clean(perc.toFixed(1)) + '%';
            } else {
                return '';
            }
        },
        insertCurrency = function () {
            if (self.options.currency) {
                return self.options.currency || '';
            }
            return '';
        },
        nFormatter = function (num, dp) {
            if (typeof dp === 'undefined' || num === 0) {
                dp = 1;
            }

            if (num == null || isNaN(num)) {
                return num;
            }

            if (num >= 1E9) {
                return clean((num / 1E9).toFixed(1)) + 'B';
            }
            if (num >= 1E6) {
                return clean((num / 1E6).toFixed(1)) + 'M';
            }
            if (num >= 1E3) {
                return clean((num / 1E3).toFixed(1)) + 'K';
            }

            return clean(num.toFixed(dp));
        },
        onChartLoaded = function (chart) {
            self = self || this;
            self.chart = chart;
            
            $.getJSON(self.options.url, function (data) {

                debugger;
                data.title = ''; //data.title || null;
                self.chart.series[0].name = data.title;
                self.chart.yAxis[0].setTitle({ text: data.title }, true);
                receivedData(data, false);
            }
            );

            var interval = setInterval(updateChartData, self.options.updateInterval);
        },
        plotSeries = function (seriesIndex, data, name) {
            var series = self.chart.series[seriesIndex * 1];
            var shift = true;
            var render = false;

            if (series.data.length < 20) {
                shift = false;
                render = true;
            }

            var lastExistingXValue = 0;
            if (series.processedXData && series.processedXData.length) {
                //lastExistingXValue = series.processedXData[series.processedXData.length - 1];
            }

            if (series) {
                if ($.isArray(data)) {
                    if (data.length) {
                        if ($.isArray(data[0])) {
                            for (var i = 0; i < data.length; i++) {
                                var pt = data[i];
                                if (lastUpdated == pt[0]) {
                                    // Updating the final value.
                                    var last = series.data[series.data.length - 1];
                                    last.update(pt[1], (i === data.length - 1));
                                } else {
                                    series.addPoint(pt, (i === data.length - 1), shift);
                                }
                                lastUpdated = pt[0];
                            }
                        } else {
                            series.addPoint(data, render, shift);
                            lastUpdated = data;
                        }
                    }
                } else {
                    series.addPoint(data, render, shift);
                    lastUpdated = data;
                }
            }
        },
        receivedData = function (data, isUpdate) {
            var val;

            if (typeof onDataReceived === 'function') {
                onDataReceived(data);
            }

            if (data.data) {
                if (data.data.series) {
                    var i = 0;
                    for (var s in data.data.series) {
                        plotSeries(i, data.data.series[s], s);
                        i++;
                    }
                } else {
                    plotSeries(0, data.data, '');
                }
                $('#loading').hide();
                val = null;


                switch (self.options.chartType) {
                    case self.CHARTTYPES.simple:
                        break;
                    case self.CHARTTYPES.overlaid:
                    case self.CHARTTYPES.right:
                        var series = self.chart.series[0];
                        var points = series.data;
                        if (self.options.summed) {
                            var index = 0;
                            for (index = 0; index < points.length; index++) {
                                val += points[index].y;
                            }
                        } else {
                            val = points[points.length - 1].y;
                        }

                        if (typeof val !== "undefined" && val !== null) {
                            val = Math.round(val);
                            if (self.options.formatValue) {
                                val = self.numberFormatter(val);
                            }
                            self.displayValue.html(self.insertCurrency(self.options.currency) + val);
                        }
                        break;

                }

            }
        },

        updateChartData = function () {
            $.getJSON(self.options.url + "&start=" + lastUpdated, function (data) {
                receivedData(data, true);
            }
            );
        };

    return {
        onChartLoaded: onChartLoaded,
        insertCurrency: insertCurrency,
        numberFormatter: nFormatter
    };
} ();






